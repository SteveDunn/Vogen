using System.Runtime.CompilerServices;
using Dapper;
using SmallTests.DeserializationValidationTests;

namespace SmallTests;

public static class ModuleInitialization
{
    [ModuleInitializer]
    public static void Init()
    {
        SqlMapper.AddTypeHandler(new MyVoInt_should_not_bypass_validation.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new MyVoString_should_not_bypass_validation.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new MyVoString_should_bypass_validation.DapperTypeHandler());
    }
}