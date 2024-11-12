using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Shared;
using Vogen;

namespace AnalyzerTests;

public class MarkerClassTests
{
    public class Attributes_must_reference_value_objects
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
        public async Task Errors_when_attributes_reference_non_vos()
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
            return;

            static void Validate(ImmutableArray<Diagnostic> diagnostics)
            {
                diagnostics.Should().HaveCount(2);
                diagnostics.Should().AllSatisfy(x => x.Id.Should().Be("VOG031"));
                
                diagnostics.Where(
                        d => d.GetMessage() ==
                             "Marker class 'EfCoreConverters' specifies a target value object 'NotAValueObject', but that is not a value object")
                    .Should()
                    .HaveCount(1);

                diagnostics.Where(
                        d => d.GetMessage() ==
                             "Marker class 'EfCoreConverters' specifies a target value object 'NotAValueObject2', but that is not a value object")
                    .Should()
                    .HaveCount(1);
            }
        }
    }

    public class Attributes_must_reference_value_objects_that_have_explicitly_specified_underlying_primitive
    {
        [Fact]
        public async Task OK_when_they_all_reference_vos()
        {
            var source = """
                         using System;
                         using Vogen;

                         [ValueObject<int>]
                         public partial struct GoodVo;

                         [ValueObject]
                         public partial struct BadVo;

                         [EfCoreConverter<GoodVo>]
                         [EfCoreConverter<BadVo>]
                         public partial class EfCoreConverters;
                         """;

            await new TestRunner<ValueObjectGenerator>()
                .WithSource(source)
                .ValidateWith(Validate)
                .RunOn(TargetFramework.Net8_0);
            return;

            static void Validate(ImmutableArray<Diagnostic> diagnostics)
            {
                diagnostics.Should().HaveCount(1);
                diagnostics.Should().AllSatisfy(x => x.Id.Should().Be("VOG035"));
                
                diagnostics.Where(
                        d => d.GetMessage() ==
                             "Marker class 'EfCoreConverters' specifies a target value object 'BadVo', but that type does not explicitly specify the primitive type that it wraps")
                    .Should()
                    .HaveCount(1);
            }
            
        }

        [Fact]
        public async Task Errors_when_attributes_reference_non_vos()
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
            return;

            static void Validate(ImmutableArray<Diagnostic> diagnostics)
            {
                diagnostics.Should().HaveCount(2);
                diagnostics.Should().AllSatisfy(x => x.Id.Should().Be("VOG031"));
                
                diagnostics.Where(
                        d => d.GetMessage() ==
                             "Marker class 'EfCoreConverters' specifies a target value object 'NotAValueObject', but that is not a value object")
                    .Should()
                    .HaveCount(1);

                diagnostics.Where(
                        d => d.GetMessage() ==
                             "Marker class 'EfCoreConverters' specifies a target value object 'NotAValueObject2', but that is not a value object")
                    .Should()
                    .HaveCount(1);
            }
        }
    }
}