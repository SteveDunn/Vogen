using FluentAssertions;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace Vogen.Tests.DiagnosticsTests.GlobalConfig.Happy;

[UsesVerify] 
public class HappyTests
{
    private readonly ITestOutputHelper _output;

    public HappyTests(ITestOutputHelper output) => _output = output;
    
    [Fact]
    public void Type_override()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(underlyingType: typeof(float))]


namespace Whatever;

[ValueObject]
public partial struct CustomerId
{
}";
        
        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();
    }

    [Fact]
    public void Exception_override()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(throws: typeof(Whatever.MyValidationException))]

namespace Whatever;

[ValueObject]
public partial struct CustomerId
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}

public class MyValidationException : Exception
{
    public MyValidationException(string message) : base(message) { }
}
";

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(0);
    }

    [Fact]
    public void Conversion_and_exceptions_override()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(conversions: Conversions.DapperTypeHandler, throws: typeof(Whatever.MyValidationException))]


namespace Whatever;

[ValueObject]
public partial struct CustomerId
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}


public class MyValidationException : Exception
{
    public MyValidationException(string message) : base(message) { }
}
";
        
        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(0);
    }

    [Fact]
    public void Override_all()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(underlyingType: typeof(string), conversions: Conversions.None, throws:typeof(Whatever.MyValidationException))]

namespace Whatever;

[ValueObject]
public partial struct CustomerId
{
    private static Validation Validate(string value) => value.Length > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}

public class MyValidationException : Exception
{
    public MyValidationException(string message) : base(message) { }
}
";
        
        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();
    }
}
