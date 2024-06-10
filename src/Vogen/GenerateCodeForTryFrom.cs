using System.Text;
using Microsoft.CodeAnalysis;

namespace Vogen;

public static class GenerateCodeForTryFrom
{
    public static string GenerateForAClass(VoWorkItem item, SyntaxToken className, string itemUnderlyingType)
        => Generate(item, className, itemUnderlyingType, includeNullCheck: true);

    public static string GenerateForAStruct(VoWorkItem item, SyntaxToken className, string itemUnderlyingType)
        => Generate(item, className, itemUnderlyingType, includeNullCheck: false);

    private static string Generate(VoWorkItem item, SyntaxToken className, string itemUnderlyingType, bool includeNullCheck)
    {

        if (item.Config.TryFromGeneration == TryFromGeneration.Omit)
        {
            return string.Empty;
        }

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
                        public static bool TryFrom({{itemUnderlyingType}} value, {{Util.GenerateNotNullWhenTrueAttribute()}} out {{className}} vo)
                        {
                            {{(includeNullCheck ? GenerateNullCheckAndReturnFalseIfNeeded(item) : string.Empty)}}
                            {{Util.GenerateCallToNormalizeMethodIfNeeded(item)}}
                        
                            {{Util.GenerateCallToValidationAndReturnFalseIfNeeded(item)}}
                        
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
                        /// <returns>A <see cref="ValueObjectOrError{T}"/> containing either the value object, or an error.</returns>
                        public static ValueObjectOrError<{{className}}> TryFrom({{itemUnderlyingType}} value)
                        {
                            {{(includeNullCheck ? GenerateNullCheckAndReturnErrorIfNeeded(item, className) : string.Empty)}}
                        
                            {{Util.GenerateCallToNormalizeMethodIfNeeded(item)}}
                        
                            {{Util.GenerateCallToValidationAndReturnValueObjectOrErrorIfNeeded(className, item)}}
                        
                            return new ValueObjectOrError<{{className}}>(new {{className}}(value));
                        }
                        """);
        }

        return sb.ToString();
    }
    
    
    private static string GenerateNullCheckAndReturnFalseIfNeeded(VoWorkItem voWorkItem) =>
        voWorkItem.IsTheUnderlyingAValueType ? string.Empty
            : $@"            if (value is null)
            {{
                vo = default;
                return false;
            }}
";

    private static string GenerateNullCheckAndReturnErrorIfNeeded(VoWorkItem item, SyntaxToken className) =>
        item.IsTheUnderlyingAValueType ? string.Empty
            : $@"            if (value is null)
            {{
                return new ValueObjectOrError<{className}>(Validation.Invalid(""The value provided was null""));
            }}
";

}