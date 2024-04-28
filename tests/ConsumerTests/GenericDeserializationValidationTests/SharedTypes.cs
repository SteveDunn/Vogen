using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using LinqToDB.Mapping;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ConsumerTests.GenericDeserializationValidationTests;

#region Value Objects
[ValueObject<int>(Conversions.DapperTypeHandler | Conversions.EfCoreValueConverter | Conversions.LinqToDbValueConverter | Conversions.NewtonsoftJson | Conversions.SystemTextJson | Conversions.TypeConverter)]
public partial class MyVoInt_should_not_bypass_validation
{
    private static Validation validate(int value)
    {
        if (value > 0)
            return Validation.Ok;

        return Validation.Invalid("must be greater than zero");
    }
}

[ValueObject<string>(Conversions.DapperTypeHandler | Conversions.EfCoreValueConverter | Conversions.LinqToDbValueConverter | Conversions.NewtonsoftJson | Conversions.SystemTextJson | Conversions.TypeConverter)]
public partial class MyVoString_should_not_bypass_validation
{
    private static Validation validate(string value)
    {
        if (value.Length > 10)
            return Validation.Ok;

        return Validation.Invalid("length must be greater than ten characters");
    }
}

[ValueObject<string>(Conversions.DapperTypeHandler | Conversions.EfCoreValueConverter | Conversions.LinqToDbValueConverter | Conversions.NewtonsoftJson | Conversions.SystemTextJson | Conversions.TypeConverter, deserializationStrictness: DeserializationStrictness.AllowAnything)]
public partial class MyVoString_should_bypass_validation
{
    private static Validation validate(string value)
    {
        if (value.Length > 10)
            return Validation.Ok;

        return Validation.Invalid("length must be greater than ten characters");
    }
}
#endregion

#region DBContext
public class DeserializationValidationDbContext : DbContext
{
    public DbSet<DeserializationValidationTestIntEntity>? IntEntities { get; set; }
    public DbSet<DeserializationValidationTestStringEntity>? StringEntities { get; set; }


    public DeserializationValidationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<DeserializationValidationTestIntEntity>(builder =>
            {
                builder
                    .Property(x => x.Id)
                    .HasConversion(new MyVoInt_should_not_bypass_validation.EfCoreValueConverter())
                    .ValueGeneratedNever();
            });
        modelBuilder
            .Entity<DeserializationValidationTestStringEntity>(builder =>
            {
                builder
                    .Property(x => x.Id)
                    .HasConversion(new MyVoString_should_not_bypass_validation.EfCoreValueConverter())
                    .ValueGeneratedNever();
            });
    }
}

public class DeserializationValidationDataConnection : DataConnection
{
    // public ITable<DeserializationValidationTestIntEntity> IntEntities => GetTable<DeserializationValidationTestIntEntity>();
    // public ITable<DeserializationValidationTestStringEntity> StringEntities => GetTable<DeserializationValidationTestStringEntity>();

    public DeserializationValidationDataConnection(SqliteConnection connection)
        : base(
              SQLiteTools.GetDataProvider("SQLite.MS"),
              connection,
              disposeConnection: false)
    { }
}
#endregion

#region Entities
#region EF
public class DeserializationValidationTestIntEntity
{
    public MyVoInt_should_not_bypass_validation? Id { get; set; }
}

public class DeserializationValidationTestStringEntity
{
    public MyVoString_should_not_bypass_validation? Id { get; set; }
}
#endregion

#region LinqToDB
public class DeserializationValidationTestLinqToDbTestIntEntity
{
    [Column(DataType = DataType.Int32)]
    [ValueConverter(ConverterType = typeof(MyVoInt_should_not_bypass_validation.LinqToDbValueConverter))]
    public MyVoInt_should_not_bypass_validation? Id { get; set; }
}

public class DeserializationValidationTestLinqToDbTestStringEntity
{
    [Column(DataType = DataType.VarChar)]
    [ValueConverter(ConverterType = typeof(MyVoString_should_not_bypass_validation.LinqToDbValueConverter))]
    public MyVoString_should_not_bypass_validation? Id { get; set; }
}
#endregion
#endregion
