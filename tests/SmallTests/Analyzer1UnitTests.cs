using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Vogen;
using VerifyCS = Analyzer1.Test.CSharpCodeFixVerifier<Vogen.Analyzers.Analyzer1Analyzer, Vogen.CodeFixers.Analyzer1CodeFixProvider>;

namespace Analyzer1.Test
{
    [TestClass]
    public class Analyzer1UnitTest
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task TestMethod1()
        {
            var test = @"";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public async Task TestMethod2()
        {
            var input = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Vogen;

namespace ConsoleApplication1
{
    [ValueObject]
    public partial class {|#0:TypeName|}
    {   
    }
}
";

            var output = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Vogen;

namespace ConsoleApplication1
{
    [ValueObject]
    public partial class TypeName
    {
        private static Validation Validate(int input)
        {
            return input > 0 ? Validation.Ok : Validation.Invalid(""!!"");
        }
    }
}
";

            var expected =
                VerifyCS.Diagnostic("Analyzer1").WithSeverity(DiagnosticSeverity.Info).WithLocation(0).WithArguments("TypeName");
            
            var loc = typeof(ValueObjectAttribute).Assembly.Location;

            var referenceAssemblies = ReferenceAssemblies.Default
                .AddAssemblies(
                    ImmutableArray.Create("Vogen", "Vogen.SharedTypes", loc.Replace(".dll", string.Empty))
                );
            
            var test = new VerifyCS.Test
            {
                TestState =
                {
                    Sources = {input },
                    //ExpectedDiagnostics = {expected},
                }, 

                CompilerDiagnostics = CompilerDiagnostics.Suggestions,
                //DisabledDiagnostics.Add() = new List<string>() { "CS1591"},
                // DisabledDiagnostics = { "CS1591" },
                //DiagnosticVerifier = (d,a,x) => d.Id == "Analyzer1",
                ReferenceAssemblies = referenceAssemblies,
                FixedCode = output,
                ExpectedDiagnostics = { expected }
            };


            //test.TestState.ExpectedDiagnostics.Add(expected);
            test.DisabledDiagnostics.Add("CS1591");
            await test.RunAsync();
            
            //await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}
