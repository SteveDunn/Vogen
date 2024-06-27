namespace ConsumerTests;

public class SkippableIfNotBuiltWithNoValidationFlagFactAttribute : SkippableFactAttribute
{
    public SkippableIfNotBuiltWithNoValidationFlagFactAttribute() : base()
    {
#if !VOGEN_NO_VALIDATION
        Skip = "Skipping because we're not in VOGEN_NO_VALIDATION mode";
#endif
    }
}

public class SkippableIfBuiltWithNoValidationFlagFactAttribute : SkippableFactAttribute
{
    public SkippableIfBuiltWithNoValidationFlagFactAttribute() : base()
    {
#if VOGEN_NO_VALIDATION
        Skip = "Skipping because we're not in VOGEN_NO_VALIDATION mode";
#endif
    }
}