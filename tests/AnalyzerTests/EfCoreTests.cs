using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Shared;
using VerifyXunit;
using Vogen;

namespace AnalyzerTests;

public class EfCoreTests
{
    public class Marker_class_attributes_must_reference_value_objects
    {
        [Fact]
        public async Task OK_when_they_all_reference_vos()
        {
            var source = """
                         using System;
                         using Vogen;

                         [ValueObject<int>]
                         public partial struct Vo1;

                         [EfCoreConverter<Vo1>]
                         public partial class EfCoreConverters;
                         """;

            await new TestRunner<ValueObjectGenerator>()
                .WithSource(source)
                .ValidateWith(diagnostics => diagnostics.Should().BeEmpty())
                .RunOn(TargetFramework.Net8_0);
        }

        [Fact]
        public async Task Errors_on_markers_that_are_not_vos()
        {
            var source = """
                         using System;
                         using Vogen;

                         public class NotAValueObject;
                         public class NotAValueObject2;

                         [ValueObject<int>]
                         public partial struct Vo1;

                         [EfCoreConverter<Vo1>]
                         [EfCoreConverter<NotAValueObject>]
                         [EfCoreConverter<NotAValueObject2>]
                         public partial class EfCoreConverters;
                         """;

            await new TestRunner<ValueObjectGenerator>()
                .WithSource(source)
                .ValidateWith(Validate)
                .RunOn(TargetFramework.Net8_0);

            static void Validate(ImmutableArray<Diagnostic> diagnostics)
            {
                diagnostics.Should().HaveCount(2);
                diagnostics.Should().AllSatisfy(x => x.Id.Should().Be("VOG031"));
                diagnostics.Where(
                        d => d.GetMessage() ==
                             "Type 'EfCoreConverters' specifies a target value object of NotAValueObject but it is not a value object")
                    .Should()
                    .HaveCount(1);

                diagnostics.Where(
                        d => d.GetMessage() ==
                             "Type 'EfCoreConverters' specifies a target value object of NotAValueObject2 but it is not a value object")
                    .Should()
                    .HaveCount(1);
            }
        }
    }
}