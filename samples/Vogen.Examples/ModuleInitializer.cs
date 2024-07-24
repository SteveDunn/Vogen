using System;
using System.Runtime.CompilerServices;
using Dapper;
using LinqToDB.Mapping;
using Vogen;
using Vogen.Examples.Types;

[assembly: VogenDefaults(staticAbstractsGeneration: StaticAbstractsGeneration.MostCommon, conversions: Conversions.Default | Conversions.Bson)]

namespace Vogen.Examples;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        MappingSchema.Default.SetConverter<DateTime, TimeOnly>(dt => TimeOnly.FromDateTime(dt));
        SqlMapper.AddTypeHandler(new DapperDateTimeOffsetVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperIntVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperStringVo.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperFloatVo.DapperTypeHandler());
    }
}