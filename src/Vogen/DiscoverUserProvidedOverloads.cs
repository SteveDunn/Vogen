using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Vogen;

internal class DiscoverUserProvidedOverloads
{
    public static UserProvidedOverloads Discover(INamedTypeSymbol vo, INamedTypeSymbol underlyingType)
    {
        var equalsForUnderlying = MethodDiscovery.TryGetEqualsMethod(vo, underlyingType);
        var equalsForWrapper = MethodDiscovery.TryGetEqualsMethod(vo, vo);
        
        return new UserProvidedOverloads
        {
            ToStringInfo = HasToStringOverload(vo),
            ToStringMethods = DiscoverParseMethods(vo, underlyingType),

            HashCodeInfo = HasGetHashCodeOverload(vo),

            // EqualsForWrapper = HasUserGeneratedEqualsForWrapper(vo, underlyingType),
            EqualsForWrapper = new UserProvidedEqualsForWrapper(WasProvided: equalsForWrapper is not null),
            
            // EqualsForUnderlying = HasUserGeneratedEqualsForUnderlying(vo, underlyingType),
            EqualsForUnderlying = new UserProvidedEqualsForUnderlying(WasProvided: equalsForUnderlying is not null) ,

            ParseMethods = DiscoverParseMethods(vo, underlyingType),
            TryParseMethods = DiscoverTryParseMethods(vo)
        };
    }

    private static UserProvidedParseMethods DiscoverParseMethods(
        INamedTypeSymbol wrapperType, 
        INamedTypeSymbol underlyingType)
    {
        return new UserProvidedParseMethods(
            MethodDiscovery.TryGetUserSuppliedParseMethods(wrapperType, underlyingType).ToList());
    }

    private static UserProvidedTryParseMethods DiscoverTryParseMethods(INamedTypeSymbol wrapperType)
    {
        return new UserProvidedTryParseMethods(
            MethodDiscovery.TryGetUserSuppliedTryParseMethods(wrapperType).ToList());
    }

    private static UserProvidedToString HasToStringOverload(ITypeSymbol typeSymbol)
    {
        IMethodSymbol? method = MethodDiscovery.TryGetToStringOverrides(typeSymbol);
        return method is null
            ? UserProvidedToString.NotProvided
            : new UserProvidedToString(
                WasSupplied: true,
                IsRecordClass: typeSymbol is { IsRecord: true, IsReferenceType: true },
                IsSealed: method.IsSealed,
                method);
    }

    private static UserProvidedGetHashCode HasGetHashCodeOverload(ITypeSymbol typeSymbol) => 
        new(WasProvided: MethodDiscovery.TryGetHashCodeOverload(typeSymbol) is not null);
}

public record struct UserProvidedToString(bool WasSupplied, bool IsRecordClass, bool IsSealed, IMethodSymbol? Method)
{
    public static readonly UserProvidedToString NotProvided = new(false, false, false, null);
}

public record struct UserProvidedGetHashCode(bool WasProvided);

public record struct UserProvidedEqualsForWrapper(bool WasProvided);

public record struct UserProvidedEqualsForUnderlying(bool WasProvided);

/// <summary>
/// Represents the Parse methods that the user supplied.
/// Every item is guaranteed to be static, named 'Parse', and returns the
/// same type as the wrapper (so there's no need to check return types).
/// </summary>
public class UserProvidedParseMethods : IEnumerable<IMethodSymbol>
{
    private readonly List<IMethodSymbol> _userMethods;

    public UserProvidedParseMethods(List<IMethodSymbol> userMethods) => _userMethods = userMethods;

    public bool Contains(IMethodSymbol methodFromPrimitive)
    {
        foreach (var eachUserMethod in _userMethods)
        {
            if (HasSameParameters(eachUserMethod, methodFromPrimitive))
            {
                return true;
            }
        }

        return false;

        static bool HasSameParameters(IMethodSymbol usersMethod, IMethodSymbol methodFromPrimitive)
        {
            var usersMethodParameterCount = usersMethod.Parameters.Length;

            if (usersMethodParameterCount != methodFromPrimitive.Parameters.Length)
            {
                return false;
            }

            for (int i = 0; i < usersMethodParameterCount; i++)
            {
                IParameterSymbol available = methodFromPrimitive.Parameters[i];
                IParameterSymbol provided = usersMethod.Parameters[i];

                if (!SymbolEqualityComparer.Default.Equals(available.Type, provided.Type))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public IEnumerator<IMethodSymbol> GetEnumerator() => _userMethods.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// Represents the TryParse methods that the user supplied.
/// Every item is guaranteed to be static, named 'TryParse', returns a bool,
/// and has a last parameter of the same type as the wrapper.
/// </summary>
public class UserProvidedTryParseMethods : IEnumerable<IMethodSymbol>
{
    private readonly List<IMethodSymbol> _userMethods;

    public UserProvidedTryParseMethods(List<IMethodSymbol> userMethods) => _userMethods = userMethods;

    /// <summary>
    /// Sees if the items held contains the method from the primitive.
    /// </summary>
    /// <param name="methodFromPrimitive"></param>
    /// <param name="vo"></param>
    /// <returns></returns>
    public bool Contains(IMethodSymbol methodFromPrimitive, VoWorkItem vo)
    {
        foreach (var eachUserMethod in _userMethods)
        {
            if (HasSameParameters(eachUserMethod))
            {
                return true;
            }
        }

        return false;

        bool HasSameParameters(IMethodSymbol usersMethod)
        {
            var usersMethodParameterCount = usersMethod.Parameters.Length;

            if (usersMethodParameterCount != methodFromPrimitive.Parameters.Length)
            {
                return false;
            }

            for (int i = 0; i < usersMethodParameterCount; i++)
            {
                IParameterSymbol primitiveParameter = methodFromPrimitive.Parameters[i];
                IParameterSymbol userParameter = usersMethod.Parameters[i];

                // if it's the last parameter, then it's an out parameter, so we see if it's the
                // wrapper type or the primitive type
                if (i == usersMethodParameterCount - 1)
                {
                    if(SameTypeAndDirection(primitiveParameter, userParameter, vo.UnderlyingType))
                    {
                        return true;
                    }
                }

                if (!SameTypeAndDirection(primitiveParameter, userParameter, userParameter.Type))
                {
                    return false;
                }
            }

            return true;
        }
    }

    private static bool SameTypeAndDirection(
        IParameterSymbol primitiveParameter, 
        IParameterSymbol userParameter, 
        ITypeSymbol expectedType)
    {
        bool sameType = SymbolEqualityComparer.Default.Equals(primitiveParameter.Type, expectedType);
        bool sameDirection = primitiveParameter.RefKind == userParameter.RefKind;

        return sameType && sameDirection;
    }

    public IEnumerator<IMethodSymbol> GetEnumerator() => _userMethods.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}