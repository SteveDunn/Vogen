# Integration with Entity Framework Core

It is possible to use value objects in EFCore.
Using VO structs is straightforward, and no converter is required.
Using VO classes requires generating a converter.

There are two ways of generating a converter. One is to generate it in the same project as the VO by adding the `EFCoreValueConverter` conversion in the attribute, e.g.

```c#
[ValueObject<string>(conversions: Conversions.EfCoreValueConverter)]
public partial class Name
{
    public static readonly Name NotSet = new("[NOT_SET]");
}
```

Another way, if you're using .NET 8 or greater, is to use `EfCoreConverter` attributes:

```c#
[EfCoreConverter<Domain.CustomerId>]
[EfCoreConverter<Domain.CustomerName>]
public partial class VogenEfCoreConverters;
```

This allows you to create the generator in a separate project,
which is useful if you're using something like Onion architecture,
where you don't want your domain objects to reference infrastructure code.

Now the converters are generated, in your database context, you then specify the conversion:

```c#
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<SomeEntity>(b =>
        {
            b.Property(e => e.Name)
                .HasConversion(new Name.EfCoreValueConverter());
        });
    }
```


An [EFCore example is included in the source](https://github.com/SteveDunn/Vogen/tree/main/samples/Vogen.Examples/SerializationAndConversion/EFCore).

Below is a walkthrough of that sample.

The sample uses EFCore to read and write entities to an in-memory database.
The entities contain Value Objects for the fields and a Value Object for the primary key.
It looks like this:

```c#
public class SomeEntity
{
    // // must be null in order for EF core to generate a value
    public SomeId Id { get; set; } = null!; 
    
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

// no converter needed because it's a struct of a supported type
[ValueObject<int>]
public partial struct Age
{
}

// converter needed because it's a class
[ValueObject<string>(conversions: Conversions.EfCoreValueConverter)]
public partial class Name
{
    public static readonly Name NotSet = new("[NOT_SET]");
}
```

The database context for this entity sets various things on the fields:

```c#
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<SomeEntity>(b =>
        {
            b.HasKey(x => x.Id);
            
            b.Property(e => e.Id)
                .HasValueGenerator<SomeIdValueGenerator>();
            
            b.Property(e => e.Id).HasVogenConversion();
            
            b.Property(e => e.Name).HasVogenConversion();
        });
    }
```

The `HasVogenConversion` method is an extension method that is generated. It looks similar to this:
```C#
public static class __IdEfCoreExtensions 
{
    public static PropertyBuilder<Id> HasVogenConversion(
        this PropertyBuilder<Id> propertyBuilder) =>
            propertyBuilder.HasConversion<
                Id.EfCoreValueConverter, 
                Id.EfCoreValueComparer>();
}
```

For the `Id` field, because it's being used as a primary key, it needs to be a class because EFCore compares it to null to determine if it should be auto-generated. When it is null, EFCore will use the specified `SomeIdValueGenerator`, which looks like:



```c#
internal class SomeIdValueGenerator : ValueGenerator<SomeId>
{
    public override SomeId Next(EntityEntry entry)
    {
        var entities = ((SomeDbContext)entry.Context).SomeEntities;
        
        var next = Math.Max(
            maxFrom(entities.Local), 
            maxFrom(entities)) + 1;
        
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

* There are concurrency concerns with `ValueGenerator` if multiple threads are allowed to insert at the same time. 
It is not recommended to share the DB Context across threads.
  If concurrent creation is required, then a Guid is the preferred method of a primary, auto-generated key.

* There are a few hoops to jump though, especially for primary keys.
  Value Objects are primarily used to represent 'domain concepts,'
  and while they can be coerced into living in the 'infrastructure' layer, i.e., databases, they're not a natural fit.
 An alternative is to use an 'anti-corruption layer' to translate between the infrastructure and domain;
  it's a layer for converting/mapping/validation. 
Yes, it's more code, but it's explicit.
  
<note title="Users' tips">
<a href="efcore-tips.md" summary="Handy tips for working with EF Core">This page</a> has some handy tips provided by the community.

</note>