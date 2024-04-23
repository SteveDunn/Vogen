using @double;
using @bool.@byte.@short.@float.@object;
using Vogen.Tests.Types;
// ReSharper disable EqualExpressionComparison
// ReSharper disable MemberCanBeFileLocal

namespace ConsumerTests.ServiceStackTextTests;

[ValueObject(typeof(int))]
public partial struct MyStructInt { }

#if NET7_0_OR_GREATER
[ValueObject<int>]
public partial struct MyGenericStructInt { }
#endif

[ValueObject(typeof(int))]
public partial struct MyStructInt2 { }

[ValueObject(typeof(int))]
public partial class MyClassInt { }

[ValueObject(typeof(int))]
public partial record class MyRecordClassInt { }

[ValueObject(typeof(int))]
public partial record class MyRecordClassInt2 { }

[ValueObject(typeof(int))]
public partial class MyClassInt2 { }

public class Tests
{

}