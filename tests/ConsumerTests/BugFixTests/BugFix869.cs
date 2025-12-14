namespace NuGet
{
    public class Type1;
}

namespace ConsumerTests
{
    using Type1 = global::NuGet.Type1;
    using Vogen;

    public class NuGet;

    [ValueObject<Type1>]
    internal partial record class MyVo
    {
        public static MyVo DefaultInstance => From(new Type1());
    }
}