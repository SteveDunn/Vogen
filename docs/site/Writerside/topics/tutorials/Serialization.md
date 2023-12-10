# Serializing and persisting

By default, each VO is decorated with a `TypeConverter` and `System.Text.Json` (STJ) serializer. There are other converters/serializer for:

* Newtonsoft.Json (NSJ)
* Dapper
* EFCore
* [LINQ to DB](https://github.com/linq2db/linq2db)
* protobuf-net (see the FAQ section below)

They are controlled by the `Conversions` enum. The following has serializers for NSJ and STJ:

```c#
[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(float))]
public readonly partial struct Celsius { }
```

If you don't want any conversions, then specify `Conversions.None`.

If you want your own conversion, then again specify none, and implement them yourself, just like any other type.  But be aware that even serializers will get the same compilation errors for `new` and `default` when trying to create VOs.

If you want to use Dapper, remember to register it—something like this:

```c#
SqlMapper.AddTypeHandler(new Customer.DapperTypeHandler());
```

See the examples folder for more information.
