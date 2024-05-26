using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Vogen;

namespace AnalyzerTests;

public class ToStringOverrideTests
{
    [Theory]
    [InlineData("public partial record class")]
    [InlineData("public partial record")]
    public async Task WithRecordsThatHaveNoSealedOverride_OutputErrors(string type)
    {
        var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
{type} CustomerId 
{{
    public override string ToString() => string.Empty;
}}
";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(Validate)
            .RunOnAllFrameworks();

        static void Validate(ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Should().HaveCount(1);
            Diagnostic diagnostic = diagnostics.Single();

            diagnostic.Id.Should().Be("VOG020");
            diagnostic.ToString().Should().Match(
                "*: error VOG020: ToString overrides should be sealed on records. See https://github.com/SteveDunn/Vogen/wiki/Records#tostring for more information.");
        }
    }

    [Theory]
    [InlineData("public partial record class")]
    [InlineData("public partial record")]
    public async Task WithRecordsThatDoHaveSealedOverride_DoNotOutputErrors(string type)
    {
        var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
{type} CustomerId 
{{
    public override sealed string ToString() => string.Empty;
}}
";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(d => d.Should().HaveCount(0))
            .RunOnAllFrameworks();
    }
    
    [Fact]
    public async Task RecordStruct_DoesNotRequireSealedToString()
    {
        var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
public partial record struct CustomerId 
{{
    public override string ToString() => string.Empty;
}}
";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(d => d.Should().HaveCount(0))
            .RunOnAllFrameworks();
    }

    [Theory]
    [ClassData(typeof(Types))]
    public async Task WithNonRecordsThatHaveMixtureOfSealedAndNonSealedOverrides_DoNotOutputErrors(string type, string sealedOrNot)
    {
        var source = $@"using Vogen;

namespace Whatever;

[ValueObject]
{type} CustomerId 
{{
    public override {sealedOrNot} string ToString() => string.Empty;
}}
";

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(d => d.Should().HaveCount(0))
            .RunOnAllFrameworks();
    }
    
    private class Types : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (string type in _types)
            {
                yield return
                [
                    type,
                    "sealed"
                ];

                yield return
                [
                    type,
                    string.Empty
                ];
            }
        }

        private readonly string[] _types =
        {
            "public partial class",
            "internal partial class"
        };

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
}