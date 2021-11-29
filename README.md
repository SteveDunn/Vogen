# Vogen
<p align="center">
  <img src="./assets/cavey.png">
</p>

# A cure for StringlyTyped software and PrimitiveObsession.

Primitive Obsession AKA **StringlyTyped** means being obsessed with primitives.  It is a Code Smell that degrades the quality of software.

> "*Primitive Obsession is using primitive data types to represent domain ideas*" [#](https://wiki.c2.com/?PrimitiveObsession)

## What is the repository?

This is a semi-opinionated library which generates [Value Objects](https://wiki.c2.com/?ValueObject) that wrap simple primitives such as `int`, `string`, `double` etc. The main goal of this project is to have almost the same speed and memory performance as using primitives.

Some examples:

* instead of `int age` - we'd have `Age age`. `Age` might have validation that it couldn't be negative
* instead of `string postcode` - we'd have `Postcode postcode`. `Postcode` might have validation on the format of the text

The opinions are expressed as:

* A Value Object (VO) is constructed via a factory method named `From`, e.g. `Age.From(12)`
* A VO is equatable (`Age.From(12) == Age.From(12)`)
* A VO, if validated, is validated with a public static method named `Validate` that returns a `Validation` result
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
public partial class CustomerId 
{
}
```
That's all you need to do to switch from a primitive to a Value Object.

Here it is again with some validation

```csharp
[ValueObject(typeof(int))]
public partial class CustomerId 
{
    private static Validation Validate(int value) => 
        value > 0 ? Validation.Ok : Validation.Invalid(); 
}
```

This generates the constructor and equality code. If your type better suits a value-type, which the example above does (being an `int`), then just change `class` to `struct`:
```csharp
[ValueObject(typeof(int))]
public partial struct CustomerId 
{
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

Now, callers can't mess up the ordering or parameters and accidentally pass us a Supplier ID in place of a Customer ID.

It also means that validation **is in just one place**. You can't introduce bad objects into your domain, therefore you can assume that **in _your_ domain** every ValueObject is valid.  Handy.

# Tell me more about the Code Smell
There's a blog post [here](https://dunnhq.com/posts/2021/primitive-obsession/) that describes it in more detail.  I'll recap here:

Primitive Obsession is being *obsessed* with the *seemingly* **convenient** way that primitives, such as `ints` and `strings`, allow us to represent domain objects and ideas.

It is **this**:
``` cs
int customerId = 42
```

What's wrong with that?

A customer ID likely **cannot** be *fully* represented by an `int`.  An `int` can be negative or zero, but it's unlikely a customer ID can be. So, we have **constraints** on a customer ID.  We can't _represent_ or _enforce_ those constraints on an `int`.

So, we need some validation to ensure the **constraints** of a customer ID are met. Because it's in `int`, we can't be sure if it's been checked beforehand, so we need to check it every time we use it.  Because it's a primitive, someone might've changed the value, so even if we're 100% sure we've checked it before, it still might need checking again.

So far, we've used as an example, a customer ID of value `42`.  In C#, it may come as no surprise that "`42 == 42`" (*I haven't checked that in JavaScript!*).  But, in our **domain**, should `42` always equal `42`?  Probably not if you're comparing a Supplier ID of `42` to a Customer ID of `42`! But primitives won't help you here (remember, `42 == 42`!) Given this signature:

``` cs
public void DoSomething(int customerId, int supplierId, int amount)
```

.. the compiler won't tell you you've messed it up by accidentally swapping customer and supplier IDs.

But by using ValueObjects, that signature becomes much more strongly typed:

``` cs
public void DoSomething(CustomerId customerId, SupplierId supplierId, Amount amount)
```

Now, the caller can't mess up the ordering of parameters, and the objects themselves are guaranteed to be valid and immutable.

-----

todo: need to tidy up everything below

Notes for the new source generated version below - needs tidying up:


# Why?
We want value semantics with validation, but this currently isn't part of C# / the framework.

But why not use `record`? A `record` doesn't enforce validation. A `record struct Foo(int Value)` has no validation, so you must use the constructor syntax.

# Attributes
`[ValueObject(typeof(int))]`
`int` can be any primivive, e.g. `double`, `decimal`, `string`

`[Instance(name: "Default", value: "42")]`
Means that a public static readonly property is created named as specified with a value as specified.
e.g.

```
[ValueObject(typeof(int))]
[Instance(name: "Invalid", value: -1)]
[Instance(name: "Unspecified", value: -2)]
public partial class MyInt {}
```

... generates these fields:

```csharp
public static MyInt Invalid = new MyInt(-1);

public static MyInt Unspecified = new MyInt(-2);
```

# Benchmarking
## How do I run the benchmarks?
`dotnet run -c Release -- --job short --filter *`

## Common scenario - underlying type of int with validation
This benchmark compared using an int natively (`int n = 1`) vs using a VO struct (`struct n {}`), vs using a VO class (`class n {}`).
Each uses validation that `n > 0`
``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.22000
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET Core SDK=6.0.100
  [Host]   : .NET Core 5.0.12 (CoreCLR 5.0.1221.52207, CoreFX 5.0.1221.52207), X64 RyuJIT
  ShortRun : .NET Core 5.0.12 (CoreCLR 5.0.1221.52207, CoreFX 5.0.1221.52207), X64 RyuJIT

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
|                  Method |     Mean |    Error |   StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------ |---------:|---------:|---------:|------:|-------:|------:|------:|----------:|
|        UsingIntNatively | 17.04 ns | 0.253 ns | 0.014 ns |  1.00 |      - |     - |     - |         - |
|  UsingValueObjectStruct | 19.76 ns | 2.463 ns | 0.135 ns |  1.16 |      - |     - |     - |         - |
| UsingValueObjectAsClass | 28.22 ns | 3.440 ns | 0.189 ns |  1.66 | 0.0043 |     - |     - |      72 B |

This looks very promising as the results between native int and VO struct are cloase and there is no memory overhead.

The next most common scenario is using a VO class to represent a native `String`.  These results are:

``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.22000
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET Core SDK=6.0.100
  [Host]   : .NET Core 5.0.12 (CoreCLR 5.0.1221.52207, CoreFX 5.0.1221.52207), X64 RyuJIT
  ShortRun : .NET Core 5.0.12 (CoreCLR 5.0.1221.52207, CoreFX 5.0.1221.52207), X64 RyuJIT

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
|                   Method |     Mean |    Error |  StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------- |---------:|---------:|--------:|------:|-------:|------:|------:|----------:|
|      UsingStringNatively | 204.4 ns |  8.09 ns | 0.44 ns |  1.00 | 0.0153 |     - |     - |     256 B |
|  UsingValueObjectAsClass | 250.7 ns | 29.97 ns | 1.64 ns |  1.23 | 0.0196 |     - |     - |     328 B |
| UsingValueObjectAsStruct | 248.9 ns | 18.82 ns | 1.03 ns |  1.22 | 0.0181 |     - |     - |     304 B |


## FAQ
If I reference the generator assembly itself, why do I get errors in NCrunch?

You need to set `Instrument output assembly` to `false` for the generator in the NCrunch configuration.