using System.Text;
using Microsoft.CodeAnalysis;

namespace Vogen;

public static class GenerateCodeForTryFrom
{
    public static string GenerateForAClass(VoWorkItem item, SyntaxToken className, string itemUnderlyingType)
        => Generate(item, className, itemUnderlyingType);

    public static string GenerateForAStruct(VoWorkItem item, SyntaxToken className, string itemUnderlyingType)
        => Generate(item, className, itemUnderlyingType);

    private static string Generate(VoWorkItem item, SyntaxToken className, string itemUnderlyingType)
    {
        if (item.Config.TryFromGeneration == TryFromGeneration.Omit)
        {
            return string.Empty;
        }

        string underlyingNullable = item.Nullable.QuestionMarkForUnderlying;

        StringBuilder sb = new();

        if (item.Config.TryFromGeneration is TryFromGeneration.GenerateBoolMethod or TryFromGeneration.GenerateBoolAndErrorOrMethods)
        {
            sb.Append($$"""
                        /// <summary>
                        /// Tries to build an instance from the provided underlying type.
                        /// If a normalization method is provided, it will be called.
                        /// If validation is provided, and it fails, false will be returned.
                        /// </summary>
                        /// <param name="value">The underlying type.</param>
                        /// <param name="vo">An instance of the value object.</param>
                        /// <returns>True if the value object can be built, otherwise false.</returns>
                        #pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member because of nullability attributes.
                        public static bool TryFrom({{Util.GenerateNotNullWhenTrueAttribute()}} {{itemUnderlyingType}}{{underlyingNullable}} value, {{Util.GenerateMaybeNullWhenFalse()}} out {{className}} vo)
                        #pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member because of nullability attributes.
                        {
                            {{GenerateNullCheckAndReturnFalseIfNeeded(item)}}
                            {{Util.GenerateCallToNormalizeMethodIfNeeded(item)}}
                        
                            {{GenerateCodeForCallingValidation.CallAndReturnFalseIfNeeded(item)}}
                        
                            vo = new {{className}}(value);
                        
                            return true;
                        }
                        """);
        }

        if (item.Config.TryFromGeneration is TryFromGeneration.GenerateErrorOrMethod or TryFromGeneration.GenerateBoolAndErrorOrMethods)
        {
            sb.Append($$"""
                        /// <summary>
                        /// Tries to build an instance from the provided underlying value.
                        /// If a normalization method is provided, it will be called.
                        /// If validation is provided, and it fails, an error will be returned.
                        /// </summary>
                        /// <param name="value">The primitive value.</param>
                        /// <returns>A <see cref="Vogen.ValueObjectOrError{T}"/> containing either the value object, or an error.</returns>
                        public static Vogen.ValueObjectOrError<{{className}}> TryFrom({{itemUnderlyingType}} value)
                        {
                            {{GenerateNullCheckAndReturnErrorIfNeeded(className, item)}}
                        
                            {{Util.GenerateCallToNormalizeMethodIfNeeded(item)}}
                        
                            {{GenerateCodeForCallingValidation.CallAndReturnValueObjectOrErrorIfNeeded(className, item)}}
                        
                            return new Vogen.ValueObjectOrError<{{className}}>(new {{className}}(value));
                        }
                        """);
        }

        return sb.ToString();
    }


    private static string GenerateNullCheckAndReturnFalseIfNeeded(VoWorkItem item)
    {
        if (item.IsTheUnderlyingAValueType)
        {
            return string.Empty;
        }

        return $$"""
               if (value is null)
               {
                   vo = default{{item.Nullable.BangForWrapper}};
                   return false;
               }
               """;
    }

    private static string GenerateNullCheckAndReturnErrorIfNeeded(SyntaxToken className, VoWorkItem item)
    {
        if (item.IsTheUnderlyingAValueType)
        {
            return string.Empty;
        }

        return $$"""
                 if (value is null)
                 {
                     return new Vogen.ValueObjectOrError<{{className}}>(Vogen.Validation.Invalid("The value provided was null"));
                 }
                 """;
    }
}