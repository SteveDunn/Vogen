using System.Runtime.CompilerServices;
using Dapper;

namespace SmallTests;

public static class ModuleInitialization
{
    [ModuleInitializer]
    public static void Init()
    {
        SqlMapper.AddTypeHandler(new SmallTests.DeserializationValidationTests.MyVoInt_should_not_bypass_validation.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new SmallTests.DeserializationValidationTests.MyVoString_should_not_bypass_validation.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new SmallTests.DeserializationValidationTests.MyVoString_should_bypass_validation.DapperTypeHandler());

#if NET7_0_OR_GREATER
        SqlMapper.AddTypeHandler(new SmallTests.GenericDeserializationValidationTests.MyVoInt_should_not_bypass_validation.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new SmallTests.GenericDeserializationValidationTests.MyVoString_should_not_bypass_validation.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new SmallTests.GenericDeserializationValidationTests.MyVoString_should_bypass_validation.DapperTypeHandler());
#endif
    }
}