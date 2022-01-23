using System.Runtime.CompilerServices;
using Dapper;
using VerifyTests;
using Vogen.IntegrationTests.TestTypes;

namespace Vogen.IntegrationTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
        SqlMapper.AddTypeHandler(new DapperFooVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperCharVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperBoolVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperByteVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperDateTimeOffsetVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperDateTimeVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperIntVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperStringVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperLongVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperShortVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperFloatVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperDoubleVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperDecimalVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperGuidVo.DapperTypeHandler());
    }
}