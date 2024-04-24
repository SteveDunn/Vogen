using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Vogen.Examples.SerializationAndConversion.EFCore;

internal class SomeDbContext : DbContext
{
    public DbSet<SomeEntity> SomeEntities { get; set; } = default!;

    // you can use this method explicitly when creating your entities, or use SomeIdValueGenerator as shown below
    // public int GetNextMyEntityId()
    // {
    //     var maxLocalId = SomeEntities.Local.Any() ? SomeEntities.Local.Max(e => e.Id.Value) : 0;
    //     var maxSavedId = SomeEntities.Any() ? SomeEntities.Max(e => e.Id.Value) : 0;
    //     return Math.Max(maxLocalId, maxSavedId) + 1;
    // }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<SomeEntity>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(e => e.Id).HasValueGenerator<SomeIdValueGenerator>();
            b.Property(e => e.Id).HasConversion(new SomeId.EfCoreValueConverter());
            b.Property(e => e.Name).HasConversion(new Name.EfCoreValueConverter());
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("SomeDB");
    }
}

[ValueObject(conversions: Conversions.EfCoreValueConverter)]
public partial class SomeId
{
}

// no converter needed because it's a struct of a support type
[ValueObject]
public partial struct Age
{
}

// converter needed because it's a class
[ValueObject<string>(conversions: Conversions.EfCoreValueConverter)]
[Instance("NotSet", "[NOT_SET]")]
public partial class Name
{
}

public class SomeEntity
{
    public SomeId Id { get; set; } = null!; // must be null in order for EF core to generate a value
    public Name Name { get; set; } = Name.NotSet;
    public Age Age { get; set; }
}

internal class SomeIdValueGenerator : ValueGenerator<SomeId>
{
    public override SomeId Next(EntityEntry entry)
    {
        var entities = ((SomeDbContext)entry.Context).SomeEntities;

        var next = Math.Max(MaxFrom(entities.Local), MaxFrom(entities)) + 1;

        return SomeId.From(next);

        static int MaxFrom(IEnumerable<SomeEntity> es)
        {
            return es.Any() ? es.Max(e => e.Id.Value) : 0;
        }
    }

    public override bool GeneratesTemporaryValues => false;
}