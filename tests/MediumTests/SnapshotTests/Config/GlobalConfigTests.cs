﻿using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using Vogen;
using Xunit;

namespace MediumTests.SnapshotTests.Config;

[UsesVerify] 
public class GlobalConfigTests
{
    [Fact]
    public Task Type_override()
    {
        var source = @"using System;
using Vogen;

[assembly: VogenDefaults(underlyingType: typeof(float))]


namespace Whatever;

[ValueObject]
public partial struct CustomerId
{
}";
        
        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();
        
        return Verifier.Verify(output).UseDirectory("Snapshots");
    }

    [Fact]
    public Task Exception_override()
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

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(0);

        return Verifier.Verify(output).UseDirectory("Snapshots");
    }

    [Fact]
    public Task Conversion_and_exceptions_override()
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
        
        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(0);

        return Verifier.Verify(output).UseDirectory("Snapshots");
    }

    [Fact]
    public Task Override_all()
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
        
        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();
        
        return Verifier.Verify(output).UseDirectory("Snapshots");
    }
}
