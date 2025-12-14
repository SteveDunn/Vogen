namespace NuGet
{
    public class Type1;
    public class Type2;
}

namespace ConsumerTests
{
    using PackageIdentity = global::NuGet.Type1;
    using Type2 = global::NuGet.Type2;
    using Vogen;

    public class NuGet
    {
    }

    [ValueObject<ValueTuple<PackageIdentity, Type2>>]
    internal partial record class PackageMetadata
    {
        public static PackageMetadata NuGet => From((new PackageIdentity(),new Type2()));
        public PackageIdentity Identity => Value.Item1;
    }
}