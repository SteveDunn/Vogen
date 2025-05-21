using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using VerifyCS = AnalyzerTests.Verifiers.CSharpAnalyzerVerifier<Vogen.Rules.DoNotThrowFromUserCodeAnalyzer>;

namespace AnalyzerTests;

public class DoNotThrowFromUserCodeAnalyzerTests
{
    [Fact]
    public async Task Does_not_raise_for_empty_code() => await Run("", []);

    [Fact]
    public async Task Does_not_raise_when_no_methods_throw()
    {
        var source = """
                     using System;
                     using Vogen;

                     namespace Whatever;

                     [ValueObject]
                     public partial struct CustomerId 
                     {
                          public static int NormalizeInput(int value) => value;
                     }
                     """;
        
        await Run(source, []);
    }

    [Fact]
    public async Task Raises_for_a_method_that_throws()
    {
        var source = """
                     using System;
                     using Vogen;

                     namespace Whatever;

                     [ValueObject]
                     public partial struct CustomerId 
                     {
                          public static int NormalizeInput(int value)
                          {
                              {|#0:throw new Exception("Oh no!");|} 
                          } 
                     }
                     """;
        
        await Run(
            source,
            WithDiagnostics("VOG032", DiagnosticSeverity.Warning, 0));
    }

    [Fact]
    public async Task Raises_for_a_method_that_throws_twice()
    {
        var source = """
using System;
using Vogen;

namespace Whatever;

[ValueObject]
public partial struct CustomerId 
{
    public static int NormalizeInput(int value)
    {
        if(1 == 1) {|#0:throw new Exception("Oh no!");|} 
        if(2 == 2) {|#1:throw new Exception("Oh no!");|} 
    } 
}
""";
        
        await Run(
            source,
            WithDiagnostics("VOG032", DiagnosticSeverity.Warning, 0, 1));
    }

    [Fact]
    public async Task Raises_for_methods_named_NormalizeInput_and_Validate()
    {
        var source = """
using System;
using Vogen;

namespace Whatever;

[ValueObject]
public partial struct CustomerId 
{
    public static int NormalizeInput(int value)
    {
        if(1 == 1) {|#0:throw new Exception("Oh no!");|} 
        if(2 == 2) {|#1:throw new Exception("Oh no!");|} 
    } 

    public static Validation Validate(int value)
    {
        if(1 == 1) {|#2:throw new Exception("Oh no!");|} 
        if(2 == 2) {|#3:throw new Exception("Oh no!");|} 
    } 
}
""";
        
        await Run(
            source,
            WithDiagnostics("VOG032", DiagnosticSeverity.Warning, 0, 1, 2, 3));
    }

    [Fact]
    public async Task Ignores_methods_that_are_not_NormalizeInput_or_Validate()
    {
        var source = """
using System;
using Vogen;

namespace Whatever;

[ValueObject]
public partial struct CustomerId 
{
    public static int NormalizeInput(int value)
    {
        if(1 == 1) {|#0:throw new Exception("Oh no!");|} 
        if(2 == 2) {|#1:throw new Exception("Oh no!");|} 
    } 

    public static void AnotherMethods(int value)
    {
        if(1 == 1) throw new Exception("Oh no!"); 
        if(2 == 2) throw new Exception("Oh no!"); 
    } 
}
""";
        
        await Run(
            source,
            WithDiagnostics("VOG032", DiagnosticSeverity.Warning, 0, 1));
    }

    [Fact]
    public async Task Ignores_methods_that_are_not_NormalizeInput_or_Validate2()
    {
        var source = """
using System;
using Vogen;

namespace Whatever;

[ValueObject]
public partial struct CustomerId 
{
    public static int NormalizeInput(int value)
    {
        if(1 == 1) {|#0:throw new Exception("Oh no!");|} 
        if(2 == 2) {|#1:throw new Exception("Oh no!");|} 
    } 

    public static void AnotherMethod(int value)
    {
        if(1 == 1) throw new Exception("Oh no!"); 
        if(2 == 2) throw new Exception("Oh no!"); 
    } 

    public static Validation Validate(int value)
    {
        if(1 == 1) {|#2:throw new Exception("Oh no!");|} 
        if(2 == 2) Throw();
        
        return Validation.Ok;
        
        void Throw() => {|#3:throw new Exception("Oh no!")|}; 
    } 
}
""";
        
        await Run(
            source,
            WithDiagnostics("VOG032", DiagnosticSeverity.Warning, 0, 1, 2, 3));
    }

    [Fact]
    public async Task Raises_for_inner_method()
    {
        var source = """
using System;
using Vogen;

namespace Whatever;

[ValueObject]
public partial struct CustomerId 
{
    public static int NormalizeInput(int value)
    {
        if(1 == 1) Throw();
        return 1;
        
        void Throw() 
        {
            {|#0:throw new Exception("Oh no!");|}
        } 
    } 
}
""";
        
        await Run(
            source,
            WithDiagnostics("VOG032", DiagnosticSeverity.Warning, 0));
    }

    [Fact]
    public async Task Raises_for_inner_method_expression()
    {
        var source = """
using System;
using Vogen;

namespace Whatever;

[ValueObject]
public partial struct CustomerId 
{
    public static int NormalizeInput(int value)
    {
        if(1 == 1) Throw();
        return 1;
        
        void Throw() => {|#0:throw new Exception("Oh no!")|}; 
    } 
}
""";
        
        await Run(
            source,
            WithDiagnostics("VOG032", DiagnosticSeverity.Warning, 0));
    }

    [Fact]
    public async Task Raises_for_expressions()
    {
        var source = """
using System;
using Vogen;

namespace Whatever;

[ValueObject]
public partial struct CustomerId 
{
    public static int NormalizeInput(int value)
    {
        var x = (value) switch
        {
           1 => {|#0:throw null!|},
           3 => 3,
           _ => {|#1:throw null!|}
        };
        
        if(x == 4) {|#2:throw new Exception();|}
        return x;
    }
}
""";
        
        await Run(
            source,
            WithDiagnostics("VOG032", DiagnosticSeverity.Warning, 0, 1, 2));
    }
    
    [Fact]
    public async Task Does_not_raise_when_method_is_not_in_a_ValueObject()
    {
        var source = """
                     using System;

                     namespace Whatever;

                     [Obsolete("Only for testing")]
                     public partial struct CustomerId
                     {
                          public static int NormalizeInput(int value)
                          {
                              {|#0:throw new Exception("Oh no!");|}
                          }
                     }
                     """;

        await Run(
            source,[]);
    }

    private static IEnumerable<DiagnosticResult> WithDiagnostics(string code, DiagnosticSeverity severity, params int[] locations)
    {
        foreach (var location in locations)
        {
            yield return VerifyCS.Diagnostic(code).WithSeverity(severity).WithLocation(location)
                .WithArguments("CustomerId");
        }
    }

    private async Task Run(string source, IEnumerable<DiagnosticResult> expected)
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

        test.ExpectedDiagnostics.AddRange(expected);

        await test.RunAsync();
    }

}