using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using Vogen.Analyzers;
using Xunit;

namespace NotSystem
{
    public static class Activator
    {
        public static T? CreateInstance<T>() => default(T);
    }
}

namespace SmallTests.DiagnosticsTests
{
    public class DisallowCreationWithReflectionTests
    {
        [Fact]
        public void Allows_using_Activate_CreateInstance_from_another_namespace()
        {
            var x = NotSystem.Activator.CreateInstance<string>();
        }
        
        
        [Theory]
        [InlineData("partial class")]
        [InlineData("partial struct")]
        [InlineData("readonly partial struct")]
        [InlineData("partial record class")]
        [InlineData("partial record struct")]
        [InlineData("readonly partial record struct")]
        public void Disallows_generic_method(string type)
        {
            var source = $@"using Vogen;
using System;

namespace Whatever;

[ValueObject(typeof(int))]
public {type} CustomerId {{ }}

var c = Activator.CreateInstance<CustomerId>();
";
        
            var (diagnostics, _) = TestHelper.GetGeneratedOutput<CreationUsingReflectionAnalyzer>(source);

            using var _ = new AssertionScope();

            diagnostics.Should().HaveCount(1);
            Diagnostic diagnostic = diagnostics.Single();

            diagnostic.Id.Should().Be("VOG025");
            diagnostic.ToString().Should().Match("*error VOG025: Type 'CustomerId' cannot be constructed via Reflection as it is prohibited.");
        }

        [Theory]
        [InlineData("partial class")]
        [InlineData("partial struct")]
        [InlineData("readonly partial struct")]
        [InlineData("partial record class")]
        [InlineData("partial record struct")]
        [InlineData("readonly partial record struct")]
        public void Disallows_non_generic_method(string type)
        {
            var source = $@"using Vogen;
using System;

namespace Whatever;

[ValueObject(typeof(int))]
public {type} CustomerId {{ }}

var c = Activator.CreateInstance(typeof(CustomerId));
";
        
            var (diagnostics, _) = TestHelper.GetGeneratedOutput<CreationUsingReflectionAnalyzer>(source);

            using var _ = new AssertionScope();

            diagnostics.Should().HaveCount(1);
            Diagnostic diagnostic = diagnostics.Single();

            diagnostic.Id.Should().Be("VOG025");
            diagnostic.ToString().Should().Match("*error VOG025: Type 'CustomerId' cannot be constructed via Reflection as it is prohibited.");
        }
    }
}