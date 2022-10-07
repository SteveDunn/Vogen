using System.Runtime.CompilerServices;
using VerifyTests;

namespace LargeTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
        
        VerifierSettings.AddScrubber(s =>
        {
            s.Replace("    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Vogen\", \"0.0.0.0\")]",
                "    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Vogen\", \"1.0.0.0\")]");
            s.Replace("    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Vogen\", \"2.0.0.0\")]",
                "    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Vogen\", \"1.0.0.0\")]");
            s.Replace("    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Vogen\", \"3.0.0.0\")]",
                "    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Vogen\", \"1.0.0.0\")]");
        });
    }
}