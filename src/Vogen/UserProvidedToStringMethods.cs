using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Vogen;

/// <summary>
/// Represents the ToString methods that the user supplied.
/// Every item is guaranteed to be non-static, named 'ToString', and returns a string.
/// </summary>
public class UserProvidedToStringMethods : IEnumerable<IMethodSymbol>
{
    private readonly List<IMethodSymbol> _userMethods;

    public int Count => _userMethods.Count;

    public UserProvidedToStringMethods(List<IMethodSymbol> userMethods) => _userMethods = userMethods;

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