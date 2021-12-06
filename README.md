# Vogen

![Build](https://github.com/stevedunn/vogen/actions/workflows/build.yaml/badge.svg) [![GitHub release](https://img.shields.io/github/release/stevedunn/vogen.svg)](https://GitHub.com/stevedunn/vogen/releases/) [![GitHub license](https://img.shields.io/github/license/stevedunn/vogen.svg)](https://github.com/SteveDunn/Vogen/blob/main/LICENSE) 
[![GitHub issues](https://img.shields.io/github/issues/Naereen/StrapDown.js.svg)](https://GitHub.com/stevedunn/vogen/issues/) [![GitHub issues-closed](https://img.shields.io/github/issues-closed/Naereen/StrapDown.js.svg)](https://GitHub.com/stevedunn/vogen/issues?q=is%3Aissue+is%3Aclosed)
[![Vogen stable version](https://badgen.net/nuget/v/vogen)](https://nuget.org/packages/vogen)

<p align="center">
  <img src="./assets/cavey.png">
</p>

# A cure for StringlyTyped software and PrimitiveObsession.

Primitive Obsession AKA **StringlyTyped** means being obsessed with primitives.  It is a Code Smell that degrades the quality of software.

> "*Primitive Obsession is using primitive data types to represent domain ideas*" [#](https://wiki.c2.com/?PrimitiveObsession)

## What is the repository?

This is a semi-opinionated library which generates [Value Objects](https://wiki.c2.com/?ValueObject) that wrap simple primitives such as `int`, `string`, `double` etc. The main goal of this project is to achieve **almost the same speed and memory performance as using primitives directly**.

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
public partial struct CustomerId 
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

Value Object allow more _strongly typed_ domain objects instead of primitives, which makes the code easier to read and enforces tighter method signatures, so instead of:

``` cs
public void DoSomething(int customerId, int supplierId, int amount)
```

we can have:

``` cs
public void DoSomething(CustomerId customerId, SupplierId supplierId, Amount amount)
```

Now, callers can't mess up the ordering of parameters and accidentally pass us a Supplier ID in place of a Customer ID.

It also means that validation **is in just one place**. You can't introduce bad objects into your domain, therefore you can assume that **in _your_ domain** every ValueObject is valid.  Handy.

# Tell me more about the Code Smell
There's a blog post [here](https://dunnhq.com/posts/2021/primitive-obsession/) that describes it in more detail.  I'll recap here:

Primitive Obsession is being *obsessed* with the *seemingly* **convenient** way that primitives, such as `ints` and `strings`, allow us to represent domain objects and ideas.

It is **this**:
``` cs
int customerId = 42
```

What's wrong with that?

A customer ID likely cannot be *fully* represented by an `int`.  An `int` can be negative or zero, but it's unlikely a customer ID can be. So, we have **constraints** on a customer ID.  We can't _represent_ or _enforce_ those constraints on an `int`.

So, we need some validation to ensure the **constraints** of a customer ID are met. Because it's in `int`, we can't be sure if it's been checked beforehand, so we need to check it every time we use it.  Because it's a primitive, someone might've changed the value, so even if we're 100% sure we've checked it before, it still might need checking again.

So far, we've used as an example, a customer ID of value `42`.  In C#, it may come as no surprise that "`42 == 42`" (*I haven't checked that in JavaScript!*).  But, in our **domain**, should `42` always equal `42`?  Probably not if you're comparing a Supplier ID of `42` to a Customer ID of `42`! But primitives won't help you here (remember, `42 == 42`!).

```csharp
(42 == 42) // true
(SuppliedId.From(42) == SupplierId.From(42)) // true
(SuppliedId.From(42) == VendorId.From(42)) // compilation error
```

But sometimes, we need to denote that a Value Object isn't valid or hasn't been set. We don't want anyone _outside_ of the object doing this as it could be used accidentally.  It's common to have `Unspecified` instances, e.g.

```csharp
public class Person
{
    public Age Age { get; } = Age.Unspecified;
}
```

We can do that with an `Instance` attribute:

```csharp
  [ValueObject(typeof(int))]
  [Instance("Unspecified", -1)]
  public readonly partial struct Age
  {
      public static Validation Validate(int value) =>
          value > 0 ? Validation.Ok : Validation.Invalid("Must be greater than zero.");
  }
```

This generates `public static Age Unspecified = new Age(-1);`.  The constructor is `private`, so only this type can (deliberately) create _invalid_ instances.

Now, when we use `Age`, our validation becomes clearer:

```csharp
public void Process(Person person) {
    if(person.Age == Age.Unspecified) {
        // age not specified.
    }
}
```

We can also specify other instance properties:

```csharp
[ValueObject(typeof(int))]
[Instance("Freezing", 0)]
[Instance("Boiling", 100)]
public readonly partial struct Centigrade
{
    public static Validation Validate(float value) =>
        value >= -273 ? Validation.Ok : Validation.Invalid("Cannot be colder than absolute zero");
}
```


# FAQ

* Why can't I just use `public record struct CustomerId(int id);`?

That doesn't give you validation. To validate `id`, you can't use the shorthand syntax. So you'd need to do:

```csharp
public record struct CustomerId
{
    public CustomerId(int id) {
        if(id <=0) throw new Exception(...)
    }
}
```

* If my VO is a `struct`, can I stop the use of `CustomerId customerId = default(CustomerId);`?

Yes. The analyser generates a compilation error.

* If my VO is a `struct`, can I stop the use of `CustomerId customerId = new(CustomerId);`?

Yes. The analyser generates a compilation error.

* If my VO is a struct, can I have my own constructor?

No. The parameterless constructor is generated automatically, and the constructor that takes the underlying value is also generated automatically.
If you add further constructors, then you will get a compilation error from the code generator [todo: describe]

* If my VO is a struct, can I have my own fields?

You could, but you'd get compiler warning [CS0282-There is no defined ordering between fields in multiple declarations of partial class or struct 'type'](https://docs.microsoft.com/en-us/dotnet/csharp/misc/cs0282)

* If I reference the generator assembly itself, why do I get errors in NCrunch?

You need to set `Instrument output assembly` to `false` for the generator in the NCrunch configuration.

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
|                  Method |     Mean |    Error |   StdDev | Ratio | Allocated |
|------------------------ |---------:|---------:|---------:|------:|----------:|
|        UsingIntNatively | 13.79 ns | 5.737 ns | 0.314 ns |  1.00 |         - |
|  UsingValueObjectStruct | 13.58 ns | 0.447 ns | 0.024 ns |  0.99 |         - |
```

This looks very promising as the results between a native int and a VO struct are almost identical and there is no memory overhead.

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

