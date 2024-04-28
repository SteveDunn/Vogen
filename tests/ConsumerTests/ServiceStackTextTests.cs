
// ReSharper disable EqualExpressionComparison
// ReSharper disable MemberCanBeFileLocal

namespace ConsumerTests.ServiceStackTextTests;

[ValueObject(typeof(int))]
public partial struct MyStructInt { }

[ValueObject<int>]
public partial struct MyGenericStructInt { }

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