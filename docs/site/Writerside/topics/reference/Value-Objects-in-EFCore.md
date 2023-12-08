# EF Core

It is possible to use Value Objects in EFCore. Using VO structs is straightforward, and no converter is required. Using VO classes requires generating a converter.
The converter is generated when you add the `EFCoreValueConverter` conversion in the attribute, e.g.

```c#
[ValueObject<string>(conversions: Conversions.EfCoreValueConverter)]
[Instance("NotSet", "[NOT_SET]")]
public partial class Name
{
}
```

In your database context, you then specify the conversion:

```c#
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<SomeEntity>(b =>
        {
            b.Property(e => e.Name).HasConversion(new Name.EfCoreValueConverter());
        });
    }
```

There is an [EFCore example in the source](https://github.com/SteveDunn/Vogen/tree/main/samples/Vogen.Examples/SerializationAndConversion/EFCore).

Below is a walkthrough of that sample.

The sample uses EFCore to read and write entities to an in-memory database. The entities contain Value Objects for the fields and also a Value Object for the primary key. It looks like this:

```c#
public class SomeEntity
{
    public SomeId Id { get; set; } = null!; // must be null in order for EF core to generate a value
    public Name Name { get; set; } = Name.NotSet;
    public Age Age { get; set; }
}
```

The individual Value Objects are:

```c#
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
```

The database context for this entity sets various things on the fields:

```c#
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
```

For the `Id` field, because it's being used as a primary key, it needs to be a class because EFCore compares it to null to determine if it should be auto-generated. When it is null, EFCore will use the specified `SomeIdValueGenerator`, which looks like:



```c#
internal class SomeIdValueGenerator : ValueGenerator<SomeId>
{
    public override SomeId Next(EntityEntry entry)
    {
        var entities = ((SomeDbContext)entry.Context).SomeEntities;
        
        var next = Math.Max(maxFrom(entities.Local), maxFrom(entities)) + 1;
        
        return SomeId.From(next);

        static int maxFrom(IEnumerable<SomeEntity> es)
        {
            return es.Any() ? es.Max(e => e.Id.Value) : 0;
        }
    }

    public override bool GeneratesTemporaryValues => false;
}
```

There are things to consider when using Value Objects in EF Core

* There are concurrency concerns with in the `ValueGenerator` if multiple threads are allowed to insert at the same time. 
From what I've read, it's not recommended to share the DB Context across threads. 
 I've also read that if concurrent creation is required, 
 then a Guid is the preferred method of a primary, auto-generated key.

* There are a few hoops to jump though, especially for primary keys.
  Value Objects are primarily used to represent 'domain concepts,'
  and while they can be coerced into living in the 'infrastructure' layer, i.e., databases, they're not a natural fit.
 I generally use an 'anti-corruption layer' to translate between the infrastructure and domain;
  it's a layer for converting/mapping/validation.
  Yes, it's more code, but it's explicit.

