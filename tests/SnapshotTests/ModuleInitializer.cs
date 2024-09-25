using System.Runtime.CompilerServices;
using VerifyTests;

namespace SnapshotTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
        
        VerifierSettings.AddScrubber(s =>
        {
            s.Replace("[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Vogen\", \"0.0.0.0\")]",
                "    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Vogen\", \"1.0.0.0\")]");
            
            s.Replace("[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Vogen\", \"5.0.0.0\")]",
                "    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Vogen\", \"1.0.0.0\")]");
        });
    }
}