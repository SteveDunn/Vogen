namespace ConsumerTests;

public class SkippableIfNotDebugFactAttribute : SkippableFactAttribute
{
    public SkippableIfNotDebugFactAttribute() : base()
    {
#if !DEBUG
        Skip = "Skipping because we're not in DEBUG mode";
#endif
    }
}