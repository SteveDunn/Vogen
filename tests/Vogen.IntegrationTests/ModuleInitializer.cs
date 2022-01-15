using System.Runtime.CompilerServices;
using Dapper;
using VerifyTests;
using Vogen.IntegrationTests.NewTests.Types;
using Vogen.IntegrationTests.SerializationAndConversionTests.Types;

namespace Vogen.IntegrationTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
        SqlMapper.AddTypeHandler(new DapperIntVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperStringVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperLongVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperShortVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperFloatVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperDoubleVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperDecimalVo.DapperTypeHandler());
    }
}