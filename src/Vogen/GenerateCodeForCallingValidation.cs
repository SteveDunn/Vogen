using Microsoft.CodeAnalysis;

namespace Vogen;

internal static class GenerateCodeForCallingValidation
{
    public static string CallWhenDeserializingAndCheckStrictnessFlag(VoWorkItem workItem, string throwingMethod = "ThrowHelper.ThrowWhenValidationFails")
    {
        if (!workItem.Config.DeserializationStrictness.HasFlag(DeserializationStrictness.RunMyValidationMethod))
        {
            return string.Empty;
        }

        return CallAndThrowIfRequired(workItem, throwingMethod);
    }

    public static string CallAndThrowIfRequired(VoWorkItem workItem, string throwingMethod =  "ThrowHelper.ThrowWhenValidationFails")
    {
        if (workItem.ValidateMethod is not null)
        {
            return $$"""
                     var validation = {{workItem.TypeToAugment.Identifier}}.{{workItem.ValidateMethod.Identifier.Value}}(value);
                     if (validation != global::Vogen.Validation.Ok)
                     {
                         {{throwingMethod}}(validation);
                     }

                     """;
        }

        return string.Empty;
    }

    public static string CallAndReturnFalseIfNeeded(VoWorkItem workItem)
    {
        if (workItem.ValidateMethod is not null)
        {
            return @$"var validation = {workItem.TypeToAugment.Identifier}.{workItem.ValidateMethod.Identifier.Value}(value);
            if (validation != global::Vogen.Validation.Ok)
            {{
                vo = default!;
                return false;
            }}
";
        }

        return string.Empty;
    }

    public static string CallAndReturnValueObjectOrErrorIfNeeded(SyntaxToken className, VoWorkItem workItem)
    {
        if (workItem.ValidateMethod is not null)
        {
            return @$"var validation = {workItem.TypeToAugment.Identifier}.{workItem.ValidateMethod.Identifier.Value}(value);
            if (validation != global::Vogen.Validation.Ok)
            {{
                return new global::Vogen.ValueObjectOrError<{className}>(validation);
            }}
";
        }

        return string.Empty;
    }
}