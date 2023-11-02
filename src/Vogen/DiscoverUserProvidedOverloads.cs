using Microsoft.CodeAnalysis;

namespace Vogen;

internal class DiscoverUserProvidedOverloads
{
    public static UserProvidedOverloads Discover(INamedTypeSymbol wrapperType, INamedTypeSymbol underlyingType) =>
        new()
        {
            ToStringInfo = HasToStringOverload(wrapperType),
            HashCodeInfo = HasGetHashCodeOverload(wrapperType),
            EqualsForWrapper = HasUserGeneratedEqualsForWrapper(wrapperType, underlyingType),
            EqualsForUnderlying = HasUserGeneratedEqualsForUnderlying(wrapperType, underlyingType)
        };
    
    private static UserProvidedToString HasToStringOverload(ITypeSymbol typeSymbol)
    {
        while (true)
        {
            var toStringMethods = typeSymbol.GetMembers("ToString").OfType<IMethodSymbol>();

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
                if (typeSymbol.IsRecord && eachMethod.IsImplicitlyDeclared)
                {
                    continue;
                }

                // In C# 10, the user can differentiate a ToString overload by making the method sealed.
                // We report back if it's sealed or not so that we can emit an error if it's not sealed.
                // The error stops another compilation error; if unsealed, the generator generates a duplicate ToString() method.
                return new UserProvidedToString(
                    WasSupplied: true,
                    IsRecordClass: typeSymbol is { IsRecord: true, IsReferenceType: true },
                    IsSealed: eachMethod.IsSealed,
                    eachMethod);
            }

            INamedTypeSymbol? baseType = typeSymbol.BaseType;

            if (baseType is null)
            {
                return UserProvidedToString.NotProvided;
            }
            
            if (CannotGoFurtherInHierarchy(baseType))
            {
                return UserProvidedToString.NotProvided;
            }

            typeSymbol = baseType;
        }
    }

    private static bool IsNotOverrideOrVirtual(IMethodSymbol eachMethod) => !eachMethod.IsOverride && !eachMethod.IsVirtual;

    private static UserProvidedGetHashCode HasGetHashCodeOverload(ITypeSymbol typeSymbol)
    {
        while (true)
        {
            var toStringMethods = typeSymbol.GetMembers("GetHashCode").OfType<IMethodSymbol>();

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
                if (typeSymbol.IsRecord && eachMethod.IsImplicitlyDeclared)
                {
                    continue;
                }

                // In C# 10, the user can differentiate a ToString overload by making the method sealed.
                // We report back if it's sealed or not so that we can emit an error if it's not sealed.
                // The error stops another compilation error; if unsealed, the generator generates a duplicate ToString() method.
                return new UserProvidedGetHashCode(
                    WasProvided: true);
            }

            INamedTypeSymbol? baseType = typeSymbol.BaseType;

            if (baseType is null)
            {
                return new UserProvidedGetHashCode(WasProvided: false);
            }
            
            if (CannotGoFurtherInHierarchy(baseType))
            {
                return new UserProvidedGetHashCode(WasProvided: false);
            }

            typeSymbol = baseType;
        }
    }

    private static UserProvidedEqualsForWrapper HasUserGeneratedEqualsForWrapper(ITypeSymbol typeSymbol, INamedTypeSymbol? wrapperType)
    {
        while (true)
        {
            var matchingMethods = typeSymbol.GetMembers("Equals").OfType<IMethodSymbol>();

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

                if (SymbolEqualityComparer.Default.Equals(onlyParameter, wrapperType))
                {
                    continue;
                }

                return new UserProvidedEqualsForWrapper(
                    WasProvided: true);
            }

            INamedTypeSymbol? baseType = typeSymbol.BaseType;

            if (baseType is null)
            {
                return new UserProvidedEqualsForWrapper(WasProvided: false);
            }

            if (CannotGoFurtherInHierarchy(baseType))
            {
                return new UserProvidedEqualsForWrapper(WasProvided: false);
            }

            typeSymbol = baseType;
        }
    }

    private static UserProvidedEqualsForUnderlying HasUserGeneratedEqualsForUnderlying(
        INamedTypeSymbol wrapperType,
        ITypeSymbol underlyingType)
    {
        while (true)
        {
            var matchingMethods = wrapperType.GetMembers("Equals").OfType<IMethodSymbol>();

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

                if (SymbolEqualityComparer.Default.Equals(onlyParameter.Type, underlyingType))
                {
                    return new UserProvidedEqualsForUnderlying(WasProvided: true);
                }
            }

            INamedTypeSymbol? baseType = underlyingType.BaseType;

            if (baseType is null)
            {
                return new UserProvidedEqualsForUnderlying(WasProvided: false);
            }

            if (CannotGoFurtherInHierarchy(baseType))
            {
                return new UserProvidedEqualsForUnderlying(WasProvided: false);
            }

            underlyingType = baseType;
        }
    }

    private static bool CannotGoFurtherInHierarchy(INamedTypeSymbol baseType) => 
        baseType.SpecialType is SpecialType.System_Object or SpecialType.System_ValueType;

    private static bool DoesNotHaveJustOneParameter(IMethodSymbol eachMethod) => eachMethod.Parameters.Length != 1;

    private static bool IsNotPublicOrProtected(IMethodSymbol eachMethod) =>
        eachMethod.DeclaredAccessibility is not (Accessibility.Public or Accessibility.Protected);
}

public record struct UserProvidedToString(bool WasSupplied, bool IsRecordClass, bool IsSealed, IMethodSymbol? Method)
{
    public static readonly UserProvidedToString NotProvided = new(false, false, false, null);
}

public record struct UserProvidedGetHashCode(bool WasProvided);

public record struct UserProvidedEqualsForWrapper(bool WasProvided);

public record struct UserProvidedEqualsForUnderlying(bool WasProvided);
