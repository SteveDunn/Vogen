# Records

Vogen supports records (`record class` and `record struct`). 

> **NOTE**: It is recommended to use a vanilla `class` or `struct` over records. The benefits of records don't really 
> apply to Vogen as its purpose is to wrap and protect a single primitive value.

For classes and structs, Vogen generates a lot of boilerplate code. But for records, some of this boilerplate code is 
already generated. This page lists the differences between records (classes and structs) and non-record classes and structs.

Things to note are:

## GetHashCode()
The generated code doesn't generate GetHashCode() as the default implementation does that.

## Equals
... for `Equals(vo left, vo right)` and `(vo left, primitive right)`

The generated code doesn't generate equals overloads as this is generated automatically by the compiler.

## ToString
Vogen overrides `ToString`. The implementation of `ToString` in `record` (`class` - not `struct`) enumerates fields and 
properties, which causes a problem in Vogen if the type is not initialized (for instance, if it's being converted 
or deserialized from JSON)

Note that if you want to override `ToString` yourself, i.e., to override what the C# compiler generates **and** to 
override what Vogen generates, then seal the method (C# 10 onwards), e.g.

`public override sealed string ToString() => "!!"`

## With
Vogen supports `with`. However, Vogen generally just has one property, `Value`, but using `with` is still supported and will still run normalization and validation

## Primary Constructors
Primary constructors can't be used. Vogen is primarily focused on wrapping a single underlying type. When a primary constructor is used, e.g.

```c#
[ValueObject]
public partial record Age(int Value1, string Value2);
```
We get the following compilation error:
`error CS8862: A constructor declared in a record with parameter list must have 'this' constructor initializer.`

That compilation error is rather cryptic, so we'll improve the analyzer to spot primary constructors and give a better error message.

Primary constructors break Vogen's constraint of everything is created via the `From` method.

## `Value` property changes
To support records, an `init` was added to the generated `Value` property which is hidden from the API as it's not intended for external use.

This was required to support the `with` concept, e.g., given the following

```c#
[ValueObject]
public partial record class MyRecord
{
}
```
Using it like this creates compilation errors:
```c#
        MyRecord r = MyRecord.From(123);

        MyRecord r2 = r with
        {
            Value = 2
        };
```

`error CS0200: Property or indexer 'MyRecord.Value' cannot be assigned to -- it is read only`

The `init` in the `Value` property can only be used via the `with` mechanism (as the `new Foo { Value = 123 }` would cause analyzer errors).

`init` does all the things that `From` does:
* null checks if needed
* run validation
* run normalization

These choices were made during the initial implementation for records. With hindsight, it would be better to have more consistency between records and non-records.

Another consideration to come in C# 12 is primary constructors for classes, and they will fit in with Vogen.
