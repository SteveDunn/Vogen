using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace Vogen.IntegrationTests.SnapshotTests;

[UsesVerify] 
public class GlobalConfigTests
{
    private readonly ITestOutputHelper _output;

    public GlobalConfigTests(ITestOutputHelper output) => _output = output;
    
    /// <summary>
    /// With no global (assembly) config, it should default to `int` for the underlying type,
    /// `ValueObjectValidationException` for the validation exception, and <see cref="Conversions"/> (Default).
    /// </summary>
    /// <returns></returns>
    [Fact]
    public Task Defaults()
    {
        // The source code to test
        var source = @"using System;
using Vogen;
namespace Whatever;

[ValueObject]
public partial struct CustomerId
{
}";
        
        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();
        
        return Verifier.Verify(output).UseDirectory("Snapshots");
    }

    /// <summary>
    /// We don't supply any global config, but override the underlying type on this VO.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public Task Local_type_override()
    {
        // The source code to test
        var source = @"using System;
using Vogen;
namespace Whatever;

[ValueObject(typeof(float))]
public partial struct CustomerId
{
}";
        
        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();
        
        return Verifier.Verify(output).UseDirectory("Snapshots");
    }

    /// <summary>
    /// We don't supply any global config, but override the exception type on this VO.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public Task Local_exception_override()
    {
        // The source code to test
        var source = @"using System;
using Vogen;
namespace Whatever;

[ValueObject(validationExceptionType: typeof(Vogen.IntegrationTests.SnapshotTests.MyValidationException))]
public partial struct CustomerId
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}";
        
        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();
        
        return Verifier.Verify(output).UseDirectory("Snapshots");
    }

    /// <summary>
    /// We don't supply any global config, but override the Conversion on this VO.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public Task Local_conversion_override()
    {
        // The source code to test
        var source = @"using System;
using Vogen;
namespace Whatever;

[ValueObject(conversions: Conversions.None)]
public partial struct CustomerId { }";
        
        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();
        
        return Verifier.Verify(output).UseDirectory("Snapshots");
    }

    /// <summary>
    /// We don't supply any global config, but override the exception and conversion this VO.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public Task Local_conversion_and_exception_override()
    {
        // The source code to test
        var source = @"using System;
using Vogen;
namespace Whatever;

[ValueObject(conversions: Conversions.DapperTypeHandler, validationExceptionType: typeof(Vogen.IntegrationTests.SnapshotTests.MyValidationException))]
public partial struct CustomerId
{
    private static Validation Validate(int value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}";
        
        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();
        
        return Verifier.Verify(output).UseDirectory("Snapshots");
    }

    /// <summary>
    /// We *do* provide global config and *do not* provide local config (on the VO). The VO
    /// will use what's specified in the global config.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public Task Override_global_config()
    {
        // The source code to test
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(underlyingType: typeof(string), conversions: Conversions.None, typeOfValidationException:typeof(Vogen.IntegrationTests.SnapshotTests.MyValidationException))]

[ValueObject]
public partial struct CustomerId
{
    private static Validation Validate(string value) => value > 0 ? Validation.Ok : Validation.Invalid(""xxxx"");
}";
        
        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();
        
        return Verifier.Verify(output).UseDirectory("Snapshots");
    }
}

[Serializable]
public class MyValidationException : Exception
{
    public MyValidationException() { }
    public MyValidationException(string message) : base(message) { }
    public MyValidationException(string message, Exception inner) : base(message, inner) { }

    protected MyValidationException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}