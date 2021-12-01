# Vogen

# What is the package?

This is a semi-opinionated library which generates [Value Objects](https://wiki.c2.com/?ValueObject) that wrap simple primitives such as `int`, `string`, `double` etc. The main goal of this project is to have almost the same speed and memory performance as using primitives.

Primitive Obsession means being obsessed with primitives.  It is a Code Smell that degrades the quality of software.

> "*Primitive Obsession is using primitive data types to represent domain ideas*" [#](https://wiki.c2.com/?PrimitiveObsession)

Some examples:

* instead of `int age` - we'd have `Age age`. `Age` might have validation that it couldn't be negative
* instead of `string postcode` - we'd have `Postcode postcode`. `Postcode` might have validation on the format of the text

The opinions are expressed as:

* A Value Object (VO) is constructed via a factory method named `From`, e.g. `Age.From(12)`
* A VO is equatable (`Age.From(12) == Age.From(12)`)
* A VO, if validated, is validated with a private static method named `Validate` that returns a `Validation` result
* Any validation that is not `Validation.Ok` results in a `ValueObjectValidationException` being thrown

Instead of

```csharp
int customerId = 42;
```

... we'd have

```csharp
var customerId = CustomerId.From(42);
```

`CustomerId` is declared as:

```csharp
[ValueObject(typeof(int))]
public partial struct CustomerId 
{
}
```
That's all you need to do to switch from a primitive to a Value Object.

Here it is again with some validation

```csharp
[ValueObject(typeof(int))]
public partial struct CustomerId 
{
    private static Validation Validate(int value) => 
        value > 0 ? Validation.Ok : Validation.Invalid(); 
}
```

This allows us to have more _strongly typed_ domain objects instead of primitives, which makes the code easier to read and enforces better method signatures, so instead of:

``` cs
public void DoSomething(int customerId, int supplierId, int amount)
```
we can have:

``` cs
public void DoSomething(CustomerId customerId, SupplierId supplierId, Amount amount)
```

Now, callers can't mess up the ordering of parameters and accidentally pass us a Supplier ID in place of a Customer ID.

It also means that validation **is in just one place**. You can't introduce bad objects into your domain, therefore you can assume that **in _your_ domain** every ValueObject is valid.  Handy.

## How does it compare to using native types?

Here's the benchmarks comparing a native int to a ValueObject:

```ini
|                  Method |     Mean |    Error |   StdDev | Ratio | Allocated |
|------------------------ |---------:|---------:|---------:|------:|----------:|
|        UsingIntNatively | 17.04 ns | 0.253 ns | 0.014 ns |  1.00 |         - |
|  UsingValueObjectStruct | 19.76 ns | 2.463 ns | 0.135 ns |  1.16 |         - |
```

There's hardly any speed overhead, and no memory overhead.

The next most common scenario is using a VO class to represent a natits are:

```ini
|                   Method |     Mean |    Error |  StdDev | Ratio | Allocated |
|------------------------- |---------:|---------:|--------:|------:|----------:|
|      UsingStringNatively | 204.4 ns |  8.09 ns | 0.44 ns |  1.00 |     256 B |
|  UsingValueObjectAsClass | 250.7 ns | 29.97 ns | 1.64 ns |  1.23 |     328 B |
| UsingValueObjectAsStruct | 248.9 ns | 18.82 ns | 1.03 ns |  1.22 |     304 B |
```