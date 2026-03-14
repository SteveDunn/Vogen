# Handling nulls

Vogen is primarily designed for the domain layer where nulls just add confusion. For instance, what does a null `SettlmentAmount` mean? Does it mean there's nothing to settle, or does it mean something else?

But sometimes, in other layers, where nulls do exist, it can be useful to have convenience methods, for instance, `FromNullable(decimal?)` or `FromNullable(int?)`.

This how-to describes how to extend Vogen with new methods using extension members in C# 14 and onwards.

In your assembly (normally in your 'Infrastructure' layer if you're using Onion/Clean Architecture), specify that Vogen creates the `IVogen<TWrapper, TPrimitive>` on all types. 

```c#
[assembly: VogenDefaults(
    staticAbstractsGeneration: StaticAbstractsGeneration.InstancesHaveInterfaceDefinition | StaticAbstractsGeneration.FactoryMethods)]
```

Now, each value object will implement `IVogen<TWrapper, TPrimitive>`, allowing you to extend it.

Now, add the following:

```c#
internal static class Extensions
{
    extension<TWrapper, TPrimitive>(IVogen<TWrapper, TPrimitive>) 
        where TWrapper : IVogen<TWrapper, TPrimitive> 
        where TPrimitive : struct
    {
        public static TWrapper? FromNullable(TPrimitive? value) => value is null ? default : TWrapper.From(value.Value);
    }
    
    extension<TWrapper, TPrimitive>(IVogen<TWrapper, TPrimitive>)
        where TWrapper : IVogen<TWrapper, TPrimitive>
        where TPrimitive : class
    {
        public static TWrapper? FromNullable(TPrimitive? value) => value is null ? default : TWrapper.From(value);
    }    
}
```

You can now have:

```c#
string? primitive = null;
CustomerName? n = CustomerName.FromNullable(primitive);
```
                        
There is a [sample provided showing how to do this](https://github.com/SteveDunn/Vogen/tree/main/samples/ExampleExtensions).
