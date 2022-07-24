using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Vogen.Analyzers;
using Xunit;

namespace SmallTests.DiagnosticsTests;

public class ToStringOverrideTests
{
    [Theory]
    [InlineData("public record struct")]
    [InlineData("public readonly record struct")]
    [InlineData("public record class")]
    [InlineData("public record")]
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
        
        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ToStringOverrideAnalyzer>(source);

        diagnostics.Should().HaveCount(1);
        Diagnostic diagnostic = diagnostics.Single();

        diagnostic.Id.Should().Be("VOG020");
        diagnostic.ToString().Should().Match("*: error VOG020: ToString overrides should be sealed on records. See https://github.com/SteveDunn/Vogen/wiki/Records#tostring for more information.");
    }

    [Theory]
    [InlineData("public record struct")]
    [InlineData("public readonly record struct")]
    [InlineData("public record class")]
    [InlineData("public record")]
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
        
        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ToStringOverrideAnalyzer>(source);

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
        
        var (diagnostics, _) = TestHelper.GetGeneratedOutput<ToStringOverrideAnalyzer>(source);

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
            "public struct",
            "public readonly struct",
            "public class",
            "internal class",
            "internal readonly struct"
        };

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
}