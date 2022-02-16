using System;
using System.Linq;
using System.Runtime.Serialization;
using FluentAssertions;
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
    public void No_creation_using_new()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(underlyingType: typeof(string), conversions: Conversions.None, typeOfValidationException:typeof(Vogen.IntegrationTests.DiagnosticsTests.BadException))]

[ValueObject(typeOfValidationException:typeof(Vogen.IntegrationTests.DiagnosticsTests.BadException))]
public partial struct CustomerId
{
    private static Validation Validate(string value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}

public class BadException{}
";

        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG010");
        diagnostic.ToString().Should().Be("(10,13): error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited.");
    }
}

public class BadException{}

[Serializable]
public class GoodException : Exception
{
    public GoodException() { }
    public GoodException(string message) : base(message) { }
    public GoodException(string message, Exception inner) : base(message, inner) { }

    protected GoodException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}