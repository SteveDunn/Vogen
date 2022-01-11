using System.Runtime.CompilerServices;
using Dapper;
using VerifyTests;
using Vogen.IntegrationTests.NewTests.Types;

namespace Vogen.IntegrationTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
        SqlMapper.AddTypeHandler(new DapperIntVo.DapperTypeHandler());
    }
}