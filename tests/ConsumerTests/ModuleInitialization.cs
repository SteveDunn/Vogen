using System.Runtime.CompilerServices;
using Dapper;
using LinqToDB.Mapping;

namespace ConsumerTests;

public static class ModuleInitialization
{
    [ModuleInitializer]
    public static void Init()
    {
        SqlMapper.AddTypeHandler(new ConsumerTests.DeserializationValidationTests.MyVoInt_should_not_bypass_validation.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new ConsumerTests.DeserializationValidationTests.MyVoString_should_not_bypass_validation.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new ConsumerTests.DeserializationValidationTests.MyVoString_should_bypass_validation.DapperTypeHandler());

#if NET6_0_OR_GREATER
        MappingSchema.Default.SetConverter<DateTime, TimeOnly>(dt => TimeOnly.FromDateTime(dt));
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.ClassVos.DapperDateOnlyVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.ClassVos.DapperTimeOnlyVo.DapperTypeHandler());
#endif

#if NET7_0_OR_GREATER
        SqlMapper.AddTypeHandler(new ConsumerTests.GenericDeserializationValidationTests.MyVoInt_should_not_bypass_validation.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new ConsumerTests.GenericDeserializationValidationTests.MyVoString_should_not_bypass_validation.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new ConsumerTests.GenericDeserializationValidationTests.MyVoString_should_bypass_validation.DapperTypeHandler());
#endif

        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.ClassVos.DapperFooVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.ClassVos.DapperCharVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.ClassVos.DapperBoolVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.ClassVos.DapperByteVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.ClassVos.DapperDateTimeOffsetVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.ClassVos.DapperDateTimeVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.ClassVos.DapperIntVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.ClassVos.DapperStringVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.ClassVos.DapperLongVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.ClassVos.DapperShortVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.ClassVos.DapperFloatVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.ClassVos.DapperDoubleVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.ClassVos.DapperDecimalVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.ClassVos.DapperGuidVo.DapperTypeHandler());

        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.StructVos.DapperFooVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.StructVos.DapperCharVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.StructVos.DapperBoolVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.StructVos.DapperByteVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.StructVos.DapperDateTimeOffsetVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.StructVos.DapperDateTimeVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.StructVos.DapperIntVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.StructVos.DapperStringVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.StructVos.DapperLongVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.StructVos.DapperShortVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.StructVos.DapperFloatVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.StructVos.DapperDoubleVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.StructVos.DapperDecimalVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.StructVos.DapperGuidVo.DapperTypeHandler());

        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordClassVos.DapperFooVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordClassVos.DapperCharVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordClassVos.DapperBoolVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordClassVos.DapperByteVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordClassVos.DapperDateTimeOffsetVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordClassVos.DapperDateTimeVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordClassVos.DapperIntVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordClassVos.DapperStringVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordClassVos.DapperLongVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordClassVos.DapperShortVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordClassVos.DapperFloatVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordClassVos.DapperDoubleVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordClassVos.DapperDecimalVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordClassVos.DapperGuidVo.DapperTypeHandler());

        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordStructVos.DapperFooVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordStructVos.DapperCharVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordStructVos.DapperBoolVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordStructVos.DapperByteVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordStructVos.DapperDateTimeOffsetVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordStructVos.DapperDateTimeVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordStructVos.DapperIntVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordStructVos.DapperStringVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordStructVos.DapperLongVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordStructVos.DapperShortVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordStructVos.DapperFloatVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordStructVos.DapperDoubleVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordStructVos.DapperDecimalVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new Vogen.IntegrationTests.TestTypes.RecordStructVos.DapperGuidVo.DapperTypeHandler());

    }
}