using Microsoft.EntityFrameworkCore;

namespace Vogen.Examples.SerializationAndConversion.EFCore;

internal class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<PersonEntity> Entities { get; set; } = default!;

    // you can use this method explicitly when creating your entities, or use SomeIdValueGenerator as shown below
    // public int GetNextMyEntityId()
    // {
    //     var maxLocalId = SomeEntities.Local.Any() ? SomeEntities.Local.Max(e => e.Id.Value) : 0;
    //     var maxSavedId = SomeEntities.Any() ? SomeEntities.Max(e => e.Id.Value) : 0;
    //     return Math.Max(maxLocalId, maxSavedId) + 1;
    // }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<PersonEntity>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(e => e.Id).HasValueGenerator<SomeIdValueGenerator>();
            b.Property(e => e.Id).HasVogenConversion();
            b.Property(e => e.Name).HasVogenConversion();
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("SomeDB");
    }
}