using System;
using System.Linq;
using System.Runtime.Serialization;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace Vogen.IntegrationTests.DiagnosticsTests;

[UsesVerify] 
public class ConfigTests
{
    private readonly ITestOutputHelper _output;

    public ConfigTests(ITestOutputHelper output) => _output = output;

    [Fact]
    public void Cannot_use_non_exception_type_in_global_config()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(underlyingType: typeof(int), conversions: Conversions.None, throws:typeof(Whatever.BadException))]

namespace Whatever;

[ValueObject)]
public partial struct CustomerId
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}

public class BadException{}
";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        using AssertionScope scope = new AssertionScope();
        diagnostic.Id.Should().Be("VOG012");
        diagnostic.ToString().Should()
            .Be("(14,14): error VOG012: BadException must derive from System.Exception");
    }

    [Fact]
    public void Cannot_use_non_exception_type_in_local_config()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(underlyingType: typeof(int), conversions: Conversions.None, throws:typeof(Vogen.ValueObjectValidationException))]

namespace Whatever;

[ValueObject(throws:typeof(Whatever.BadException))]
public partial struct CustomerId
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}

public class BadException{}
";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        using AssertionScope scope = new AssertionScope();
        diagnostic.Id.Should().Be("VOG012");
        diagnostic.ToString().Should()
            .Be("(14,14): error VOG012: BadException must derive from System.Exception");
    }

    [Fact]
    public void Cannot_use_non_exception_type_in_local_and_global_config()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(underlyingType: typeof(int), conversions: Conversions.None, throws:typeof(BadException1))]

namespace Whatever;

[ValueObject(throws:typeof(BadException2))]
public partial struct CustomerId
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}

public class BadException1{} // this should break compilation
public class BadException2{} // this should break compilation
public class BadException_that_is_not_mentioned_in_any_attribute{} // this one is OK
";

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(2);

        using AssertionScope scope = new AssertionScope();
        diagnostics.Should().AllSatisfy(d => d.Id.Should().Be("VOG012"));

        diagnostics.ElementAt(0).ToString().Should()
            .Be("error VOG012: BadException1 must derive from System.Exception");
        diagnostics.ElementAt(1).ToString().Should()
            .Be("(15,14): error VOG012: BadException2 must derive from System.Exception");
    }

    [Fact]
    public void Cannot_use_custom_exception_without_a_constructor_with_one_parameter()
    {
        var source = @"using System;
using Vogen;

namespace Whatever;

[ValueObject(throws:typeof(BadException))]
public partial struct CustomerId
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}

public class BadException : Exception { } // breaks because there's no constructor
";

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(1);

        using AssertionScope scope = new AssertionScope();
        diagnostics[0].Id.Should().Be("VOG013");
        diagnostics[0].ToString().Should().Be("(12,14): error VOG013: BadException must have at least 1 constructor with 1 parameter");
    }
}

// [Serializable]
// public class GoodException : Exception
// {
//     public GoodException() { }
//     public GoodException(string message) : base(message) { }
//     public GoodException(string message, Exception inner) : base(message, inner) { }
//
//     protected GoodException(
//         SerializationInfo info,
//         StreamingContext context) : base(info, context)
//     {
//     }
// }