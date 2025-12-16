namespace NuGet
{
    public class Type1;
}

namespace ConsumerTests.BugFixTests.BugFix869
{
    using Type1 = global::NuGet.Type1;

    public class NuGet;
    public class System;
    public class Microsoft;
    public class NuGet<T>;
    public class System<T>;
    public class Microsoft<T>;
    public class Vogen;
    public class Vogen<T>;

    [global::Vogen.ValueObject<Type1>]
    internal partial record class MyVo
    {
        public static MyVo DefaultInstance => From(new Type1());
        public static MyVo NuGet => From(new Type1());
    }
}