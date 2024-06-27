namespace ConsumerTests;

public class SkippableIfNotReleaseFactAttribute : SkippableFactAttribute
{
    public SkippableIfNotReleaseFactAttribute() : base()
    {
#if !RELEASE
        Skip = "Skipping because we're not in RELEASE mode";
#endif
    }
}