using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using SmallTests.AnalyzerTests;
using VerifyCS = AnalyzerTests.Verifiers.CSharpCodeFixVerifier<Vogen.Rules.ValidationMethodAnalyzer, Vogen.Rules.ValidateMethodFixers.AddMethodCodeFixProvider>;

namespace AnalyzerTests.ValidationMethodAnalysis;

public class ValidationMethodAnalyzerTests
{
    //No diagnostics expected to show up
    [Fact]
    public async Task NoDiagnosticsForEmptyCode() => await VerifyCS.VerifyAnalyzerAsync(string.Empty);

    //Diagnostic and CodeFix both triggered and checked for
    [Fact]
    public async Task Not_triggered_if_method_exists()
    {
        var input = LineEndingsHelper.Normalize("""
using System;
using Vogen;

namespace ConsoleApplication1;
[ValueObject(typeof(int))]
public partial class {|#0:TypeName|}
{   
    private static Validation Validate(int input)
    {
        bool isValid = true; // todo: your validation
        return isValid ? Validation.Ok : Validation.Invalid("[todo: describe the validation]");
    }
}
""");

        var test = new VerifyCS.Test
        {
            TestState =
            {
                Sources = { input }
            },

            CompilerDiagnostics = CompilerDiagnostics.Suggestions,
            ReferenceAssemblies = References.Net80AndOurs.Value,
        };

        test.DisabledDiagnostics.Add("CS1591");

        await test.RunAsync();
    }

    [Fact]
    public async Task Not_triggered_if_method_exists_with_fqn_return_type()
    {
        var input = LineEndingsHelper.Normalize("""
using System;

namespace ConsoleApplication1;

[Vogen.ValueObject(typeof(int))]
public partial class {|#0:TypeName|}
{   
    private static Vogen.Validation Validate(int input)
    {
        bool isValid = true; // todo: your validation
        return isValid ? Vogen.Validation.Ok : Vogen.Validation.Invalid("[todo: describe the validation]");
    }
}
""");

        var test = new VerifyCS.Test
        {
            TestState =
            {
                Sources = { input }
            },

            CompilerDiagnostics = CompilerDiagnostics.Suggestions,
            ReferenceAssemblies = References.Net80AndOurs.Value,
        };

        test.DisabledDiagnostics.Add("CS1591");

        await test.RunAsync();
    }

    [Fact]
    public async Task Triggered_when_method_is_missing()
    {
        var input = LineEndingsHelper.Normalize("""
using System;
using Vogen;

namespace ConsoleApplication1;

[ValueObject(typeof(int))]
public partial class {|#0:TypeName|}
{   
}
""");

        var expectedOutput = LineEndingsHelper.Normalize("""
using System;
using Vogen;

namespace ConsoleApplication1;

[ValueObject(typeof(int))]
public partial class TypeName
{
    private static Validation Validate(int input)
    {
        bool isValid = true; // todo: your validation
        return isValid ? Validation.Ok : Validation.Invalid("[todo: describe the validation]");
    }
}
""");

        var expectedDiagnostic =
            VerifyCS.Diagnostic("AddValidationMethod").WithSeverity(DiagnosticSeverity.Info).WithLocation(0).WithArguments("TypeName");

        var test = new VerifyCS.Test
        {
            TestState =
            {
                Sources = { input }
            },

            CompilerDiagnostics = CompilerDiagnostics.Suggestions,
            ReferenceAssemblies = References.Net80AndOurs.Value,
            FixedCode = expectedOutput,
            ExpectedDiagnostics = { expectedDiagnostic },
        };

        test.DisabledDiagnostics.Add("CS1591");

        await test.RunAsync();
    }

    [Fact]
    public async Task Triggered_for_missing_method_and_assembly_level_config_specifies_a_different_type()
    {
        var input = LineEndingsHelper.Normalize(
            """
                #pragma warning disable CS0618 // 'Customizations.TreatNumberAsStringInSystemTextJson' is obsolete
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using System.Text;
                using System.Threading.Tasks;
                using System.Diagnostics;
                using Vogen;
            
                [assembly: VogenDefaults(
                    typeof(string),
                    Conversions.TypeConverter | Conversions.SystemTextJson,
                    customizations: Customizations.TreatNumberAsStringInSystemTextJson,
                    debuggerAttributes: DebuggerAttributeGeneration.Basic
                )]
            
                namespace ConsoleApplication1
                {
                    [ValueObject]
                    public partial class {|#0:TypeName|}
                    {
                    }
                }
            """);

        var expectedOutput = LineEndingsHelper.Normalize(
            """
                #pragma warning disable CS0618 // 'Customizations.TreatNumberAsStringInSystemTextJson' is obsolete
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using System.Text;
                using System.Threading.Tasks;
                using System.Diagnostics;
                using Vogen;
            
                [assembly: VogenDefaults(
                    typeof(string),
                    Conversions.TypeConverter | Conversions.SystemTextJson,
                    customizations: Customizations.TreatNumberAsStringInSystemTextJson,
                    debuggerAttributes: DebuggerAttributeGeneration.Basic
                )]
            
                namespace ConsoleApplication1
                {
                    [ValueObject]
                    public partial class TypeName
                    {
                    private static Validation Validate(string input)
                    {
                        bool isValid = true; // todo: your validation
                        return isValid ? Validation.Ok : Validation.Invalid("[todo: describe the validation]");
                    }
                }
                }
            """);

        var expectedDiagnostic =
            VerifyCS.Diagnostic("AddValidationMethod").WithSeverity(DiagnosticSeverity.Info).WithLocation(0).WithArguments("TypeName");

        var test = new VerifyCS.Test
        {
            TestState =
            {
                Sources = { input }
            },

            CompilerDiagnostics = CompilerDiagnostics.Suggestions,
            ReferenceAssemblies = References.Net80AndOurs.Value,
            FixedCode = expectedOutput,
            ExpectedDiagnostics = { expectedDiagnostic },
        };

        test.DisabledDiagnostics.Add("CS1591");

        await test.RunAsync();
    }

    [Fact]
    public async Task Triggered_for_missing_method_and_assembly_level_config_does_not_specify_a_different_type()
    {
        var input = LineEndingsHelper.Normalize(
            """
                #pragma warning disable CS0618 // 'Customizations.TreatNumberAsStringInSystemTextJson' is obsolete
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using System.Text;
                using System.Threading.Tasks;
                using System.Diagnostics;
                using Vogen;
            
                [assembly: VogenDefaults(
                    null,
                    Conversions.TypeConverter | Conversions.SystemTextJson,
                    customizations: Customizations.TreatNumberAsStringInSystemTextJson,
                    debuggerAttributes: DebuggerAttributeGeneration.Basic
                )]
            
                namespace ConsoleApplication1
                {
                    [ValueObject]
                    public partial class {|#0:TypeName|}
                    {
                    }
                }
            """);

        var expectedOutput = LineEndingsHelper.Normalize(
            """
                #pragma warning disable CS0618 // 'Customizations.TreatNumberAsStringInSystemTextJson' is obsolete
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using System.Text;
                using System.Threading.Tasks;
                using System.Diagnostics;
                using Vogen;
            
                [assembly: VogenDefaults(
                    null,
                    Conversions.TypeConverter | Conversions.SystemTextJson,
                    customizations: Customizations.TreatNumberAsStringInSystemTextJson,
                    debuggerAttributes: DebuggerAttributeGeneration.Basic
                )]
            
                namespace ConsoleApplication1
                {
                    [ValueObject]
                    public partial class TypeName
                    {
                    private static Validation Validate(int input)
                    {
                        bool isValid = true; // todo: your validation
                        return isValid ? Validation.Ok : Validation.Invalid("[todo: describe the validation]");
                    }
                }
                }
            """);

        var expectedDiagnostic =
            VerifyCS.Diagnostic("AddValidationMethod").WithSeverity(DiagnosticSeverity.Info).WithLocation(0).WithArguments("TypeName");

        var test = new VerifyCS.Test
        {
            TestState =
            {
                Sources = { input }
            },

            CompilerDiagnostics = CompilerDiagnostics.Suggestions,
            ReferenceAssemblies = References.Net80AndOurs.Value,
            FixedCode = expectedOutput,
            ExpectedDiagnostics = { expectedDiagnostic },
        };

        test.DisabledDiagnostics.Add("CS1591");

        await test.RunAsync();
    }

    //Diagnostic and CodeFix both triggered and checked for
    [Fact]
    public async Task Trigged_for_missing_method_and_vo_is_decorated_with_generic_attribute()
    {
        var input = LineEndingsHelper.Normalize(
            """
            using System;
            using Vogen;

            namespace ConsoleApplication1
            {
                [ValueObject<int>]
                public partial class {|#0:TypeName|}
                {   
                }
            }
            """);

        var expectedOutput = LineEndingsHelper.Normalize(
            """
            using System;
            using Vogen;

            namespace ConsoleApplication1
            {
                [ValueObject<int>]
                public partial class TypeName
                {
                    private static Validation Validate(int input)
                    {
                        bool isValid = true; // todo: your validation
                        return isValid ? Validation.Ok : Validation.Invalid("[todo: describe the validation]");
                    }
                }
            }
            """);

        var expectedDiagnostic =
            VerifyCS.Diagnostic("AddValidationMethod").WithSeverity(DiagnosticSeverity.Info).WithLocation(0).WithArguments("TypeName");

        var test = new VerifyCS.Test
        {
            TestState =
            {
                Sources = { input },
                ReferenceAssemblies = References.Net80AndOurs.Value
            },

            CompilerDiagnostics = CompilerDiagnostics.Suggestions,
            ReferenceAssemblies = References.Net80AndOurs.Value,
            FixedCode = expectedOutput,
            ExpectedDiagnostics = { expectedDiagnostic },
        };

        test.DisabledDiagnostics.Add("CS1591");

        await test.RunAsync();
    }
}