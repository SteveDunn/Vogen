using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Vogen;
using VerifyCS = AnalyzerTests.Verifiers.CSharpAnalyzerVerifier<Vogen.Rules.DoNotUseUninitializedValueObjectInPropertyAnalyzer>;
// ReSharper disable CoVariantArrayConversion

namespace AnalyzerTests;

public class DoNotUseUninitializedValueObjectInPropertyTests
{
    private class AllVoTypes : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return ["partial class"];
            yield return ["partial struct"];
            yield return ["readonly partial struct"];
            yield return ["partial record class"];
            yield return ["partial record struct"];
            yield return ["readonly partial record struct"];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Fact]
    public async Task NoDiagnosticsForEmptyCode()
    {
        await VerifyCS.VerifyAnalyzerAsync(string.Empty);
    }

    [Theory]
    [ClassData(typeof(AllVoTypes))]
    public async Task NoDiagnostic_when_property_has_initializer(string voType)
    {
        var source = $$"""
                       using Vogen;
                       namespace Whatever;

                       [ValueObject(typeof(int))]
                       public {{voType}} MyVo { }

                       public class TestClass
                       {
                           public MyVo SomeProperty { get; set; } = MyVo.From(0);
                       }

                       """;

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(d => d.Should().BeEmpty())
            .RunOnAllFrameworks(true);
    }

    [Theory]
    [ClassData(typeof(AllVoTypes))]
    public async Task NoDiagnostic_when_property_is_nullable(string voType)
    {
        var source = $$"""
                       #nullable enable
                       using Vogen;
                       namespace Whatever;

                       [ValueObject(typeof(int))]
                       public {{voType}} MyVo { }

                       public class TestClass
                       {
                           public MyVo? SomeProperty { get; set; }
                       }

                       """;

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(d => d.Should().BeEmpty())
            .RunOnAllFrameworks(true);
    }

    [Theory]
    [ClassData(typeof(AllVoTypes))]
    public async Task NoDiagnostic_when_property_is_getter_only(string voType)
    {
        var source = $$"""
                       using Vogen;
                       namespace Whatever;

                       [ValueObject(typeof(int))]
                       public {{voType}} MyVo { }

                       public class TestClass
                       {
                           public TestClass(MyVo vo) { SomeProperty = vo; }
                           public MyVo SomeProperty { get; }
                       }

                       """;

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(d => d.Should().BeEmpty())
            .RunOnAllFrameworks(true);
    }

    [Theory]
    [ClassData(typeof(AllVoTypes))]
    public async Task NoDiagnostic_when_property_is_expression_bodied(string voType)
    {
         var source = $$"""
                        using System;
                        using Vogen;
                        namespace Whatever;

                        [ValueObject(typeof(int))]
                        public {{voType}} MyVo { }

                        public class TestClass
                        {
                            private MyVo _backing = MyVo.From(1);
                            public MyVo SomeProperty => _backing;
                        }

                        """;
         
         await new TestRunner<ValueObjectGenerator>()
             .WithSource(source)
             .ValidateWith(d => d.Should().BeEmpty())
             .RunOnAllFrameworks(true);
    }

    [Theory]
    [ClassData(typeof(AllVoTypes))]
    public async Task NoDiagnostic_when_property_is_static(string voType)
    {
        var source = $$"""
                       using Vogen;
                       namespace Whatever;

                       [ValueObject(typeof(int))]
                       public {{voType}} MyVo { }

                       public class TestClass
                       {
                           public static MyVo SomeProperty { get; set; }
                       }

                       """;

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(d => d.Should().BeEmpty())
            .RunOnAllFrameworks(true);
    }

    [Theory]
    [ClassData(typeof(AllVoTypes))]
    public async Task NoDiagnostic_when_property_is_inside_a_value_object(string voType)
    {
        // A VO declaring a property of another VO type — the outer VO manages its own initialization.
        var source = $$"""
                       using Vogen;
                       namespace Whatever;

                       [ValueObject(typeof(int))]
                       public {{voType}} MyVo { }

                       [ValueObject(typeof(int))]
                       public partial class OuterVo
                       {
                           public MyVo Inner { get; set; }
                       }

                       """;

        await new TestRunner<ValueObjectGenerator>()
            .WithSource(source)
            .ValidateWith(d => d.Should().BeEmpty())
            .RunOnAllFrameworks(true);
    }

#if NET7_0_OR_GREATER
    [Theory]
    [ClassData(typeof(AllVoTypes))]
    public async Task NoDiagnostic_when_property_has_required_modifier(string voType)
    {
        var source = $$"""
                       using Vogen;
                       namespace Whatever;

                       [ValueObject(typeof(int))]
                       public {{voType}} MyVo { }

                       public class TestClass
                       {
                           public required MyVo SomeProperty { get; set; }
                       }

                       """;
        await Run(source);
        // await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Theory]
    [ClassData(typeof(AllVoTypes))]
    public async Task Diagnostic_for_required_missing_init_only_property(string voType)
    {
        var source = $$"""
                       using Vogen;
                       namespace Whatever;

                       [ValueObject(typeof(int))]
                       public {{voType}} MyVo { }

                       public class TestClass
                       {
                           {|#0:public MyVo SomeInitProperty { get; init; }|}
                       }

                       """;
        await Run(
            source,
            WithDiagnostics("VOG038", DiagnosticSeverity.Warning, "MyVo", 0));
    }
#endif

    [Theory]
    [ClassData(typeof(AllVoTypes))]
    public async Task Diagnostic_for_non_nullable_uninitialized_get_set_property(string voType)
    {
        var source = $$"""
                       using Vogen;
                       namespace Whatever;

                       [ValueObject(typeof(int))]
                       public {{voType}} MyVo { }

                       public class TestClass
                       {
                           {|#0:public MyVo SomeProperty { get; set; }|}
                       }

                       """;
        await Run(
            source,
            WithDiagnostics("VOG038", DiagnosticSeverity.Warning, "MyVo", 0));
    }

    [Theory]
    [ClassData(typeof(AllVoTypes))]
    public async Task Diagnostic_for_multiple_uninitialized_properties(string voType)
    {
        var source = $$"""
                       using Vogen;
                       namespace Whatever;

                       [ValueObject(typeof(int))]
                       public {{voType}} MyVo { }

                       [ValueObject(typeof(string))]
                       public {{voType}} MyOtherVo { }

                       public class TestClass
                       {
                           {|#0:public MyVo First { get; set; }|}
                           {|#1:public MyOtherVo Second { get; set; }|}
                       }

                       """;
        await Run(
            source,
            WithDiagnostics("VOG038", DiagnosticSeverity.Warning, "MyVo", 0),
            WithDiagnostics("VOG038", DiagnosticSeverity.Warning, "MyOtherVo", 1));
    }

    [Theory]
    [ClassData(typeof(AllVoTypes))]
    public async Task Diagnostic_when_struct_property_is_settable_but_uninitialized(string voType)
    {
        var source = $$"""
                       using Vogen;
                       namespace Whatever;

                       [ValueObject(typeof(int))]
                       public {{voType}} MyVo { }

                       public struct ContainerStruct
                       {
                           {|#0:public MyVo SomeProperty { get; set; }|}
                       }

                       """;
        await Run(
            source,
            WithDiagnostics("VOG038", DiagnosticSeverity.Warning, "MyVo", 0));
    }

    private static IEnumerable<DiagnosticResult> WithDiagnostics(string code, DiagnosticSeverity severity,
        string argument, params int[] locations)
    {
        foreach (var location in locations)
        {
            yield return VerifyCS.Diagnostic(code)
                .WithSeverity(severity)
                .WithLocation(location)
                .WithArguments(argument);
        }
    }

    private static async Task Run(string source, params IEnumerable<DiagnosticResult>[] expectedGroups)
    {
        var test = new VerifyCS.Test
        {
            TestState =
            {
                Sources = { source },
            },

            CompilerDiagnostics = CompilerDiagnostics.Errors,
            ReferenceAssemblies = References.Net90AndOurs.Value,
        };

        foreach (var group in expectedGroups)
        {
            test.ExpectedDiagnostics.AddRange(group);
        }

        await test.RunAsync();
    }
}
