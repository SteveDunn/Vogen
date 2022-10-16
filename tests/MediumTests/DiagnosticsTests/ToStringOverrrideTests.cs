using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using Vogen;
using Xunit;

namespace SmallTests.DiagnosticsTests;

public class ToStringOverrideTests
{
    [Theory]
    [InlineData("public partial record struct")]
    [InlineData("public readonly partial record struct")]
    [InlineData("public partial record class")]
    [InlineData("public partial record")]
    public void WithRecordsThatHaveNoSealedOverride_OutputErrors(string type)
    {
        var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
{type} CustomerId 
{{
    public override string ToString() => string.Empty;
}}
";
        
        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        using var _ = new AssertionScope();
        
        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG020");
        diagnostic.ToString().Should().Match("*: error VOG020: ToString overrides should be sealed on records. See https://github.com/SteveDunn/Vogen/wiki/Records#tostring for more information.");
    }

    [Theory]
    [InlineData("public partial record struct")]
    [InlineData("public readonly partial record struct")]
    [InlineData("public partial record class")]
    [InlineData("public partial record")]
    public void WithRecordsThatDoHaveSealedOverride_DoNotOutputErrors(string type)
    {
        var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
{type} CustomerId 
{{
    public override sealed string ToString() => string.Empty;
}}
";
        
        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(0);
    }

    [Theory]
    [ClassData(typeof(Types))]
    public void WithNonRecordsThatHaveMixtureOfSealedAndNonSealedOverrides_DoNotOutputErrors(string type, string sealedOrNot)
    {
        var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
{type} CustomerId 
{{
    public override {sealedOrNot} string ToString() => string.Empty;
}}
";
        
        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().HaveCount(0);
    }
    
    private class Types : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (string type in _types)
            {
                yield return new object[]
                {
                    type,
                    "sealed"
                };

                yield return new object[]
                {
                    type,
                    string.Empty
                };
            }
        }

        private readonly string[] _types =
        {
            "public partial struct",
            "public readonly partial struct",
            "public partial class",
            "internal partial class",
            "internal readonly partial struct"
        };

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
}