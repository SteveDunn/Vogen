using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Vogen;

namespace Testbench;

public class Program
{
    public static async Task Main()
    {
        SqlMapper.AddTypeHandler(new Vo.DapperTypeHandler());

        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        Vo? vo = (await connection.QueryAsync<Vo>("SELECT -1")).AsList()[0];

        Console.WriteLine(vo.Value);
    }
}

[ValueObject(typeof(int), Conversions.DapperTypeHandler, deserializationStrictness: DeserializationStrictness.AllowValidAndKnownInstances)]
[Instance(name: "Unknown", value: 0)]
//[Instance(name: "Invalid", value: -1)]
public partial class Vo
{
    private static int NormalizeInput(int input) => input == -1 ? 0 : input;

    private static Validation validate(int value) => 
        value > 0 ? Validation.Ok : Validation.Invalid("must be greater than zero");
}
