using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Vogen.EfCoreTest;

public class EmployeeEntity
{
    public Id Id { get; set; } = null!; // must be null in order for EF core to generate a value
    public required Name Name { get; set; } = Name.NotSet;
    public required Age Age { get; set; }
    
    public required Department Department { get; set; }
    
    public required HireDate HireDate { get; set; }
}

[ValueObject]
public partial class Id;

[ValueObject<string>]
[Instance("NotSet", "[NOT_SET]")]
public partial class Name;

[ValueObject]
public readonly partial struct Age;

[ValueObject<string>]
public readonly partial record struct Department;

[ValueObject<DateOnly>]
public partial record class HireDate;


internal class MyContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<EmployeeEntity> Entities { get; set; } = default!;

    // you can use this method explicitly when creating your entities, or use SomeIdValueGenerator as shown below
    // public int GetNextMyEntityId()
    // {
    //     var maxLocalId = SomeEntities.Local.Any() ? SomeEntities.Local.Max(e => e.Id.Value) : 0;
    //     var maxSavedId = SomeEntities.Any() ? SomeEntities.Max(e => e.Id.Value) : 0;
    //     return Math.Max(maxLocalId, maxSavedId) + 1;
    // }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        
        // There are two ways of registering these, you can call the generated extension method here,
        // or register the converters individually, like below in `OnModelCreating`.
        // configurationBuilder.RegisterAllInVogenEfCoreConverters2();
        // configurationBuilder.RegisterAllInVogenEfCoreConverters2();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<EmployeeEntity>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(e => e.Id).HasValueGenerator<SomeIdValueGenerator>();
            
            // There are two ways of registering these, you can do them inline here,
            // or with the `RegisterAllIn[xxx]` like above in `ConfigureConventions`
            
            b.Property(e => e.Id).HasVogenConversion();
            b.Property(e => e.Age).HasVogenConversion();
            b.Property(e => e.Name).HasVogenConversion();
            b.Property(e => e.Department).HasVogenConversion();
            b.Property(e => e.HireDate).HasVogenConversion();
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
optionsBuilder.UseSqlite("Data Source=SomeDB.db");
        //optionsBuilder.UseInMemoryDatabase("SomeDB");
    }
}

internal class SomeIdValueGenerator : ValueGenerator<Id>
{
    public override Id Next(EntityEntry entry)
    {
        var entities = ((MyContext)entry.Context).Entities;

        var next = Math.Max(MaxFrom(entities.Local), MaxFrom(entities)) + 1;

        return Id.From(next);

        static int MaxFrom(IEnumerable<EmployeeEntity> es)
        {
            return es.Any() ? es.Max(e => e.Id.Value) : 0;
        }
    }

    public override bool GeneratesTemporaryValues => false;
}
