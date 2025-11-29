using System.Threading.Tasks;
using Shared;
using Vogen;

namespace SnapshotTests.InternalDiagnostics;

public class InternalDiagnosticsTests
{
    [Fact]
    public Task It_writes_diagnostics_if___ProduceDiagnostics_class_is_present_in_the_Vogen_namespace()
    {
        return RunTest(
            """
            using System;
            using Vogen;
            
            [assembly: VogenDefaults(conversions: Conversions.None)]

            namespace MyNamespace
            {
                public static class C {
                    public static void Main() { }
                }
            
                [ValueObject]
                public partial struct MyVo { }
            }
            
            namespace Vogen
            {
                public class __ProduceDiagnostics { }
            }
            """);
        
        static Task RunTest(string source) =>
            new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
        
    }

    [Fact]
    public Task It_does_not_write_diagnostics_if___ProduceDiagnostics_class_is_not_present_in_the_Vogen_namespace()
    {
        return RunTest(
            """
            using System;
            using Vogen;
            
            [assembly: VogenDefaults(conversions: Conversions.None)]

            namespace MyNamespace
            {
                public static class C {
                    public static void Main() { }
                }
            
                [ValueObject]
                public partial struct MyVo { }

                public class __ProduceDiagnostics { }
            }
            
            namespace Vogen
            {
            }
            """);
        
        static Task RunTest(string source) =>
            new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
        
    }

    [Fact]
    public Task It_does_not_write_diagnostics_if___ProduceDiagnostics_class_is_not_present_anywhere()
    {
        return RunTest(
            """
            using System;
            using Vogen;

            namespace MyNamespace;

            [ValueObject]
            public partial struct MyVo { }
            """);
        
        static Task RunTest(string source) =>
            new SnapshotRunner<ValueObjectGenerator>()
                .WithSource(source)
                .RunOn(TargetFramework.Net8_0);
        
    }
}