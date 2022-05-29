using System.Runtime.CompilerServices;
using VerifyTests;

namespace Vogen.IntegrationTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
        
        VerifierSettings.AddScrubber(s =>
            s.Replace("    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Vogen\", \"0.0.0.0\")]",
                "    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Vogen\", \"1.0.0.0\")]"));

        // SqlMapper.AddTypeHandler(new TestTypes.ClassVos.DapperFooVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.ClassVos.DapperCharVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.ClassVos.DapperBoolVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.ClassVos.DapperByteVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.ClassVos.DapperDateTimeOffsetVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.ClassVos.DapperDateTimeVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.ClassVos.DapperIntVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.ClassVos.DapperStringVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.ClassVos.DapperLongVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.ClassVos.DapperShortVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.ClassVos.DapperFloatVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.ClassVos.DapperDoubleVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.ClassVos.DapperDecimalVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.ClassVos.DapperGuidVo.DapperTypeHandler());
        //
        // SqlMapper.AddTypeHandler(new TestTypes.StructVos.DapperFooVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.StructVos.DapperCharVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.StructVos.DapperBoolVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.StructVos.DapperByteVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.StructVos.DapperDateTimeOffsetVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.StructVos.DapperDateTimeVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.StructVos.DapperIntVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.StructVos.DapperStringVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.StructVos.DapperLongVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.StructVos.DapperShortVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.StructVos.DapperFloatVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.StructVos.DapperDoubleVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.StructVos.DapperDecimalVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.StructVos.DapperGuidVo.DapperTypeHandler());
        //
        // SqlMapper.AddTypeHandler(new TestTypes.RecordClassVos.DapperFooVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordClassVos.DapperCharVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordClassVos.DapperBoolVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordClassVos.DapperByteVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordClassVos.DapperDateTimeOffsetVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordClassVos.DapperDateTimeVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordClassVos.DapperIntVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordClassVos.DapperStringVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordClassVos.DapperLongVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordClassVos.DapperShortVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordClassVos.DapperFloatVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordClassVos.DapperDoubleVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordClassVos.DapperDecimalVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordClassVos.DapperGuidVo.DapperTypeHandler());
        //
        // SqlMapper.AddTypeHandler(new TestTypes.RecordStructVos.DapperFooVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordStructVos.DapperCharVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordStructVos.DapperBoolVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordStructVos.DapperByteVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordStructVos.DapperDateTimeOffsetVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordStructVos.DapperDateTimeVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordStructVos.DapperIntVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordStructVos.DapperStringVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordStructVos.DapperLongVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordStructVos.DapperShortVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordStructVos.DapperFloatVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordStructVos.DapperDoubleVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordStructVos.DapperDecimalVo.DapperTypeHandler());
        // SqlMapper.AddTypeHandler(new TestTypes.RecordStructVos.DapperGuidVo.DapperTypeHandler());
    }
}