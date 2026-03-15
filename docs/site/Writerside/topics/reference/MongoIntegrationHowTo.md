# Integration with MongoDB

It is possible to use value objects (VOs) in MongoDB.

To generate a converter (serializer), add the `Bson` conversion in the attribute, e.g.

```c#
[ValueObject<string>(conversions: Conversions.Bson)]
public partial class Name
{
    public static readonly Name NotSet = new("[NOT_SET]");
}
```

Now that the serializers are generated, you now need to register them.
Vogen generates a static class named `RegisterBsonSerializersFor[NameOfProject]`.

By default, this class has a static constructor that automatically registers all BSON serializers when first accessed.
However, if you need to run BSON configuration before the serializers are registered, you can use the `ManuallyRegisterBsonSerializers` customization:

```c#
[assembly: VogenDefaults(customizations: Customizations.ManuallyRegisterBsonSerializers)]
```

With this flag set, Vogen generates a `TryRegister()` method without a static constructor, allowing you to control when registration occurs:

```C#
// Call this manually after any BSON configuration
BsonSerializationRegisterFor[YourProjectName].TryRegister();
```

Without the flag, the static constructor automatically calls code like:

```C#
BsonSerializer.TryRegisterSerializer(new CustomerIdBsonSerializer());
BsonSerializer.TryRegisterSerializer(new EnergyUsedBsonSerializer());
```
A [MongoDB example is included in the source](https://github.com/SteveDunn/Vogen/tree/main/samples/Vogen.Examples/SerializationAndConversion/Mongo).

Below is a walkthrough of that sample.

The sample uses MongoDB to read and write entities (a `Person`) to a MongoDB database in a testcontainer.
Note that attributes on the value objects do not specify the BSON serializer; that is specified in global config in `ModuleInitializer.cs`:

```c#
[ValueObject]
public readonly partial struct Age;

[ValueObject<string>]
public readonly partial struct Name;

public class Person
{
    public Name Name { get; set; }
    public Age Age { get; set; }
}
```

This simple example registers the serializers manually:
```C#
BsonSerializer.RegisterSerializer(new NameBsonSerializer());
BsonSerializer.RegisterSerializer(new AgeBsonSerializer());
```

… but it could just as easily registered them with the generated register:
```C#
BsonSerializationRegisterForVogen_Examples.TryRegister();
```

(_replace `Vogen_Examples` with the name of *your* project_)

> **Note:** By default, the static class has a static constructor that automatically registers serializers when first accessed.
> If you need control over when registration happens (e.g., to configure BSON first), use the `Customizations.ManuallyRegisterBsonSerializers` flag
> to generate only the `TryRegister()` method without the static constructor.

Next, it adds a bunch of `Person` objects to the database, each containing value objects representing age and name, and then reads them back.