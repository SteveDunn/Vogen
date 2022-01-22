using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Testbench.FooTests;

public static class Tests
{
    public static void TestDapper()
    {
            SqlMapper.AddTypeHandler(new NoJsonFooVo.DapperTypeHandler());

        using var connection = new SqliteConnection("DataSource=:memory:");
         connection.Open();

        IEnumerable<NoJsonFooVo> results = connection.Query<NoJsonFooVo>("SELECT '{\"Age\":42,\"Name\":\"Fred\"}'");

        NoJsonFooVo value = results.Single();



        var bar = new Bar { Age = 12, Name = "Fred" };
        
        string serializedBar = Newtonsoft.Json.JsonConvert.SerializeObject(bar);
        var bar2 = Newtonsoft.Json.JsonConvert.DeserializeObject<Bar>(serializedBar);
        
        var foo = NoJsonFooVo.From(new Bar { Age = 12, Name = "Fred" });
        
        string serializedFoo = Newtonsoft.Json.JsonConvert.SerializeObject(foo);
        var foo2 = Newtonsoft.Json.JsonConvert.DeserializeObject<NoJsonFooVo>(serializedFoo);
    }

    public static void TestNoJson()
    {
        var bar = new Bar { Age = 12, Name = "Fred" };
        
        string serializedBar = Newtonsoft.Json.JsonConvert.SerializeObject(bar);
        var bar2 = Newtonsoft.Json.JsonConvert.DeserializeObject<Bar>(serializedBar);
        
        var foo = NoJsonFooVo.From(new Bar { Age = 12, Name = "Fred" });
        
        string serializedFoo = Newtonsoft.Json.JsonConvert.SerializeObject(foo);
        var foo2 = Newtonsoft.Json.JsonConvert.DeserializeObject<NoJsonFooVo>(serializedFoo);
    }

    public static void TestNewtonAndTypeConverter()
    {
        var bar = new Bar { Age = 12, Name = "Fred" };
        string serializedBar = Newtonsoft.Json.JsonConvert.SerializeObject(bar);
        var bar2 = Newtonsoft.Json.JsonConvert.DeserializeObject<Bar>(serializedBar);

        var foo = FooWithNewtonAndTypeConverter.From(new Bar { Age = 12, Name = "Fred" });
        string serializedFoo = Newtonsoft.Json.JsonConvert.SerializeObject(foo);
        var foo2 = Newtonsoft.Json.JsonConvert.DeserializeObject<FooWithNewtonAndTypeConverter>(serializedFoo);
    }
}