namespace NuGet
{
    public class PackageIdentity;
    public class LicenseMetadata;
    public class PackageDeprecationMetadata;
    public class PackageVulnerabilityMetadata;
}

namespace ConsumerTests
{
    using PackageIdentity = global::NuGet.PackageIdentity;
    using LicenseMetadata = global::NuGet.LicenseMetadata;
    using PackageDeprecationMetadata = global::NuGet.PackageDeprecationMetadata;
    using PackageVulnerabilityMetadata = global::NuGet.PackageVulnerabilityMetadata;
    using Vogen;

    public class NuGet
    {
    }

    [ValueObject<ValueTuple<PackageIdentity, LicenseMetadata, Uri, PackageDeprecationMetadata, PackageVulnerabilityMetadata>>]
    internal partial record class PackageMetadata
    {
        public static PackageMetadata NuGet => From((null!, null!, null!, null!, null!));
        public PackageIdentity Identity => Value.Item1;

        public LicenseMetadata License => Value.Item2;

        public Uri LicenseUrl => Value.Item3;

        public PackageDeprecationMetadata DeprecationMetadata => Value.Item4;

        public PackageVulnerabilityMetadata VulnerabilityMetadata => Value.Item5;

        private static Validation Validate(
            (PackageIdentity Identity, LicenseMetadata LicenseMetadata, Uri LicenseUrl, PackageDeprecationMetadata DeprecationMetadata,
                PackageVulnerabilityMetadata VulnerabilityMetadata) input)
            => input.Identity != null && input.LicenseMetadata != null
                ? Validation.Ok
                : Validation.Invalid($"'{input}' is not a valid {nameof(PackageMetadata)}.");
    }
}