using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Dapper;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;
using Microsoft.EntityFrameworkCore;
using LinqToDB;

namespace ConsumerTests.GenericDeserializationValidationTests;

public class StringDeserializationValidationTests
{
    [Fact]
    public async Task Deserialization_dapper_should_not_bypass_validation_pass()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var actual = (await connection.QueryAsync<MyVoString_should_not_bypass_validation>("SELECT 'abcdefghijk'")).AsList()[0].Value;

        actual.Should().Be("abcdefghijk");
    }

    [Fact]
    public async Task Deserialization_dapper_should_not_bypass_validation_fail()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        Func<Task<string>> vo = async () => (await connection.QueryAsync<MyVoString_should_not_bypass_validation>("SELECT 'abc'")).AsList()[0].Value;

        await vo.Should().ThrowExactlyAsync<ValueObjectValidationException>().WithMessage("length must be greater than ten characters");
    }

    [Fact]
    public async Task Deserialization_dapper_should_bypass_validation_fail()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

         MyVoString_should_bypass_validation? vo = (await connection.QueryAsync<MyVoString_should_bypass_validation>("SELECT 'abc'")).AsList()[0];

         vo.Value.Should().Be("abc");
    }

    [Fact]
    public async Task Deserialization_efcore_should_not_bypass_validation_pass()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<DeserializationValidationDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new DeserializationValidationDbContext(options))
        {
            var actual = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SingleAsync(context.StringEntities!.FromSqlRaw("SELECT 'abcdefghijk' As Id"));
            actual.Id!.Value.Should().Be("abcdefghijk");
        }
    }

    [Fact]
    public async Task Deserialization_efcore_should_not_bypass_validation_fail()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<DeserializationValidationDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new DeserializationValidationDbContext(options))
        {
            Func<Task<string>> vo = async () => (await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SingleAsync(context.StringEntities!.FromSqlRaw("SELECT 'abc' As Id"))).Id!.Value;
            await vo.Should().ThrowExactlyAsync<ValueObjectValidationException>().WithMessage("length must be greater than ten characters");
        }
    }
    [Fact]
    public async Task Deserialization_linqtodb_should_not_bypass_validation_pass()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        using (var context = new DeserializationValidationDataConnection(connection))
        {
            var actual = await LinqToDB.AsyncExtensions.SingleAsync(context.FromSql<DeserializationValidationTestLinqToDbTestStringEntity>("SELECT 'abcdefghijk' As Id"));
            actual.Id!.Value.Should().Be("abcdefghijk");
        }
    }

    [Fact]
    public async Task Deserialization_linqtodb_should_not_bypass_validation_fail()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        //var original = new TestEntity { Id = LinqToDbStringVo.From("foo!") };
        using (var context = new DeserializationValidationDataConnection(connection))
        {
            Func<Task<string>> vo = async () => (await LinqToDB.AsyncExtensions.SingleAsync(context.FromSql<DeserializationValidationTestLinqToDbTestStringEntity>("SELECT 'abc' As Id"))).Id!.Value;
            await vo.Should().ThrowExactlyAsync<ValueObjectValidationException>().WithMessage("length must be greater than ten characters");
        }
    }

    [Fact]
    public void TypeConversion_should_not_bypass_validation_pass()
    {
        var converter = TypeDescriptor.GetConverter(typeof(MyVoString_should_not_bypass_validation));
        var validValue = "abcdefghijk";

        var actual = ((MyVoString_should_not_bypass_validation?) converter.ConvertFrom(validValue))!.Value;

        actual.Should().Be("abcdefghijk");
    }

    [Fact]
    public void TypeConversion_should_not_bypass_validation_fail()
    {
        var converter = TypeDescriptor.GetConverter(typeof(MyVoString_should_not_bypass_validation));
        var invalidValue = "abc";

        Action vo = () => converter.ConvertFrom(invalidValue);

        vo.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("length must be greater than ten characters");
    }

    [Fact]
    public void Deserialization_systemtextjson_should_not_bypass_validation_pass()
    {
        var validValue = SystemTextJsonSerializer.Serialize(MyVoString_should_not_bypass_validation.From("abcdefghijk"));

        var actual = SystemTextJsonSerializer.Deserialize<MyVoString_should_not_bypass_validation>(validValue)!.Value;

        actual.Should().Be("abcdefghijk");
    }

    [Fact]
    public void Deserialization_systemtextjson_should_not_bypass_validation_fail()
    {
        var invalidValue = SystemTextJsonSerializer.Serialize(MyVoString_should_not_bypass_validation.From("abcdefghijk")).Replace("abcdefghijk", "abc");

        Action vo = () => SystemTextJsonSerializer.Deserialize<MyVoString_should_not_bypass_validation>(invalidValue);

        vo.Should().ThrowExactly<System.Text.Json.JsonException>()
            .WithMessage("The JSON value could not be converted to ConsumerTests.GenericDeserializationValidationTests.MyVoString_should_not_bypass_validation.*")
            .WithMessage("*Path: $ |*")
            .WithInnerException<ValueObjectValidationException>()
            .WithMessage("length must be greater than ten characters");
    }

    [Fact]
    public void Deserialization_newtonsoft_should_not_bypass_validation_pass()
    {
        var validValue = NewtonsoftJsonSerializer.SerializeObject(MyVoString_should_not_bypass_validation.From("abcdefghijk"));

        var actual = NewtonsoftJsonSerializer.DeserializeObject<MyVoString_should_not_bypass_validation>(validValue)!.Value;

        actual.Should().Be("abcdefghijk");
    }

    [Fact]
    public void Deserialization_newtonsoft_should_not_bypass_validation_fail()
    {
        var invalidValue = NewtonsoftJsonSerializer.SerializeObject(MyVoString_should_not_bypass_validation.From("abcdefghijk")).Replace("abcdefghijk", "abc");

        Func<string> vo = () => NewtonsoftJsonSerializer.DeserializeObject<MyVoString_should_not_bypass_validation>(invalidValue)!.Value;

        vo.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("length must be greater than ten characters");
    }
}
