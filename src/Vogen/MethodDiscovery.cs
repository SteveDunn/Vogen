using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Analyzer.Utilities.Extensions;
using Microsoft.CodeAnalysis;

namespace Vogen;

internal static class MethodDiscovery
{
    /// <summary>
    /// Tries to get the ToString override on the Value Object or base types.
    /// It only includes overrides or virtual methods.
    /// We ignore the implicitly generated ToStrings when a VO *is* a record or *derives from* a record, as we still want to
    /// generate one ourselves as we don't want the default behaviour.
    /// </summary>
    /// <param name="typeSymbol"></param>
    /// <returns>null if no overloads</returns>
    public static IEnumerable<IMethodSymbol> GetAnyUserProvidedToStringOverrides(ITypeSymbol typeSymbol)
    {
        while (true)
        {
            var toStringMethods = typeSymbol.GetMembers("ToString").OfType<IMethodSymbol>();

            foreach (IMethodSymbol eachMethod in toStringMethods)
            {
                if (IsNotOverrideOrVirtual(eachMethod))
                {
                    continue;
                }
                
                // can't change access rights
                if (IsNotPublicOrProtected(eachMethod))
                {
                    continue;
                }

                // records always have an implicitly declared ToString method. In C# 10, the user can differentiate this
                // by making the method sealed.
                if (typeSymbol.IsRecord && eachMethod.IsImplicitlyDeclared)
                {
                    continue;
                }

                // In C# 10, the user can differentiate a ToString overload by making the method sealed.
                // We report back if it's sealed or not so that we can emit an error if it's not sealed.
                // The error stops another compilation error; if unsealed, the generator generates a duplicate ToString() method.
                yield return eachMethod;
            }

            INamedTypeSymbol? baseType = typeSymbol.BaseType;

            if (baseType is null)
            {
                yield break;
            }

            if (CannotGoFurtherInHierarchy(baseType))
            {
                yield break;
            }

            typeSymbol = baseType;
        }
    }
    
    private static bool IsNotOverrideOrVirtual(IMethodSymbol eachMethod) => eachMethod is { IsOverride: false, IsVirtual: false };

    public static ITypeSymbol? TryGetHashCodeOverload(ITypeSymbol vo)
    {
        while (true)
        {
            var toStringMethods = vo.GetMembers("GetHashCode").OfType<IMethodSymbol>();

            foreach (IMethodSymbol eachMethod in toStringMethods)
            {
                // we could have "public virtual new string ToString() => "xxx" 
                if (IsNotOverrideOrVirtual(eachMethod))
                {
                    continue;
                }

                // can't change access rights
                if (IsNotPublicOrProtected(eachMethod))
                {
                    continue;
                }

                if (eachMethod.Parameters.Length != 0)
                {
                    continue;
                }

                // records always have an implicitly declared ToString method. In C# 10, the user can differentiate this
                // by making the method sealed.
                if (vo.IsRecord && eachMethod.IsImplicitlyDeclared)
                {
                    continue;
                }

                // In C# 10, the user can differentiate a ToString overload by making the method sealed.
                // We report back if it's sealed or not so that we can emit an error if it's not sealed.
                // The error stops another compilation error; if unsealed, the generator generates a duplicate ToString() method.
                return vo;
            }

            INamedTypeSymbol? baseType = vo.BaseType;

            if (baseType is null)
            {
                return null;
            }
            
            if (CannotGoFurtherInHierarchy(baseType))
            {
                return null;
            }

            vo = baseType;
        }
    }

    /// <summary>
    /// Tries to get all the methods named Equals on the Value Object itself or base types.
    /// </summary>
    /// <param name="vo"></param>
    /// <param name="parameterType"></param>
    /// <returns></returns>
    public static IMethodSymbol? TryGetEqualsMethod(ITypeSymbol vo, ITypeSymbol parameterType)
    {
        var matchingMethods = vo.GetMembers("Equals").OfType<IMethodSymbol>();

        foreach (IMethodSymbol eachMethod in matchingMethods)
        {
            if (eachMethod.IsImplicitlyDeclared)
            {
                continue;
            }
                
            // can't change access rights
            if (IsNotPublicOrProtected(eachMethod))
            {
                continue;
            }

            if (DoesNotHaveJustOneParameter(eachMethod))
            {
                continue;
            }

            IParameterSymbol onlyParameter = eachMethod.Parameters[0];

            if (SymbolEqualityComparer.Default.Equals(onlyParameter.Type, parameterType))
            {
                return eachMethod;
            }
        }

        INamedTypeSymbol? baseType = vo.BaseType;

        if (baseType is null)
        {
            return null;
        }

        if (CannotGoFurtherInHierarchy(baseType))
        {
            return null;
        }

        return TryGetEqualsMethod(baseType, parameterType);
    }

    /// <summary>
    /// Tries to get all the methods named 'Parse' that return either the Value Object (wrapper),
    /// or a derived type.
    /// </summary>
    /// <param name="typeSymbol"></param>
    /// <param name="underlyingType"></param>
    /// <returns></returns>
    public static IEnumerable<IMethodSymbol> TryGetUserSuppliedParseMethods(
        INamedTypeSymbol typeSymbol,
        ITypeSymbol underlyingType)
    {
        while (true)
        {
            var matchingMethods = typeSymbol.GetMembers("Parse").OfType<IMethodSymbol>();

            foreach (IMethodSymbol eachMethod in matchingMethods)
            {
                if (eachMethod.IsImplicitlyDeclared)
                {
                    continue;
                }

                // if (!DoesMethodReturnThisType(eachMethod, typeSymbol))
                // {
                //     continue;
                // }

                // can't change access rights
                if (IsNotPublicOrProtected(eachMethod))
                {
                    continue;
                }

                yield return eachMethod;
            }

            INamedTypeSymbol? baseType = underlyingType.BaseType;

            if (baseType is null)
            {
                yield break;
            }

            if (CannotGoFurtherInHierarchy(baseType))
            {
                yield break;
            }

            underlyingType = baseType;
        }
    }

    /// <summary>
    /// Tries to get all the methods named 'TryParse' that return either the Value Object (wrapper),
    /// or a derived type.
    /// It doesn't care about the type it tries to parse to as this is just used to see if
    /// the user has already supplied a TryParse when hoisting the methods up from IParsable.
    /// </summary>
    /// <param name="typeSymbol"></param>
    /// <returns></returns>
    public static IEnumerable<IMethodSymbol> TryGetUserSuppliedTryParseMethods(INamedTypeSymbol typeSymbol)
    {
        var matchingMethods = typeSymbol.GetMembers("TryParse").OfType<IMethodSymbol>();

        foreach (IMethodSymbol eachMethod in matchingMethods)
        {
            if (eachMethod.IsImplicitlyDeclared)
            {
                continue;
            }

            if (eachMethod.ReturnType.Name != nameof(Boolean))
            {
                continue;
            }

            var ps = eachMethod.GetParameters();

            if (ps.Length == 0)
            {
                continue;
            }

            // can't change access rights
            if (IsNotPublicOrProtected(eachMethod))
            {
                continue;
            }

            yield return eachMethod;
        }
    }

    public static IEnumerable<IMethodSymbol> FindTryParseMethodsOnThePrimitive(INamedTypeSymbol primitiveSymbol)
    {
        ImmutableArray<ISymbol> members = primitiveSymbol.GetMembers("TryParse");

        if (members.Length == 0) yield break;
            
        foreach (ISymbol eachMember in members)
        {
            if (eachMember is IMethodSymbol s)
            {
                var ps = s.GetParameters();

                if (s.ReturnType.Name != nameof(Boolean))
                {
                    continue;
                }

                if (!SymbolEqualityComparer.Default.Equals(ps[ps.Length-1].Type, primitiveSymbol))
                {
                    continue;
                }

                yield return s;
            }
        }
    }

    public static IEnumerable<IMethodSymbol> FindParseMethodsOnThePrimitive(INamedTypeSymbol primitiveSymbol)
    {
        ImmutableArray<ISymbol> members = primitiveSymbol.GetMembers("Parse");

        if (members.Length == 0)
        {
            yield break;
        }

        foreach (ISymbol eachMember in members)
        {
            if (eachMember is IMethodSymbol s)
            {
                if(!SymbolEqualityComparer.Default.Equals(primitiveSymbol, s.ReturnType))
                {
                    continue;
                }

                yield return s;
            }
        }
    }

    public static IEnumerable<IMethodSymbol> FindToStringMethodsOnThePrimitive(INamedTypeSymbol primitiveSymbol)
    {
        ImmutableArray<ISymbol> members = primitiveSymbol.GetMembers("ToString");

        if (members.Length == 0)
        {
            yield break;
        }

        foreach (ISymbol eachMember in members)
        {
            if (eachMember is IMethodSymbol s)
            {
                if (s.IsStatic) continue;
                if (s.ReturnType.SpecialType != SpecialType.System_String) continue;

                yield return s;
            }
        }
    }

    /// <summary>
    /// Checks to see whether the primitive implements all of the methods on the interface.
    /// A primitive that doesn't implement them all, or implements them privately in the case of 
    /// </summary>
    /// <param name="primitiveSymbol"></param>
    /// <param name="interfaceSymbol"></param>
    /// <returns></returns>
    public static bool DoesPrimitivePubliclyImplementThisInterface(INamedTypeSymbol primitiveSymbol, INamedTypeSymbol? interfaceSymbol)
    {
        if (interfaceSymbol is null)
        {
            return false;
        }
        
        if (!primitiveSymbol.DerivesFrom(interfaceSymbol))
        {
            return false;
        }
        
        ImmutableArray<ISymbol> interfaceMembers = interfaceSymbol.GetMembers();
        
        foreach (ISymbol eachInterfaceMember in interfaceMembers)
        {
            if (eachInterfaceMember.Kind == SymbolKind.Method)
            {
                if (!HasNonPrivateMethod(eachInterfaceMember, primitiveSymbol))
                {
                    return false;
                }
            }
        }
        
        return true;
    }

    private static bool HasNonPrivateMethod(ISymbol soughtMethod, INamedTypeSymbol symbol)
    {
        ISymbol? implementation = symbol.FindImplementationForInterfaceMember(soughtMethod);

        return implementation?.DeclaredAccessibility is not Accessibility.Private;
    }

    private static bool IsNotVirtual(IMethodSymbol eachMethod) => eachMethod is { IsVirtual: false };

    private static bool CannotGoFurtherInHierarchy(INamedTypeSymbol baseType) => 
        baseType.SpecialType is SpecialType.System_Object or SpecialType.System_ValueType;

    private static bool DoesNotHaveJustOneParameter(IMethodSymbol eachMethod) => eachMethod.Parameters.Length != 1;

    private static bool IsNotPublicOrProtected(IMethodSymbol eachMethod) =>
        eachMethod.DeclaredAccessibility is not (Accessibility.Public or Accessibility.Protected);
}