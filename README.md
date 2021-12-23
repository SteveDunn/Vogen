![Build](https://github.com/stevedunn/vogen/actions/workflows/build.yaml/badge.svg) [![GitHub release](https://img.shields.io/github/release/stevedunn/vogen.svg)](https://GitHub.com/stevedunn/vogen/releases/) [![GitHub license](https://img.shields.io/github/license/stevedunn/vogen.svg)](https://github.com/SteveDunn/Vogen/blob/main/LICENSE) 
[![GitHub issues](https://img.shields.io/github/issues/Naereen/StrapDown.js.svg)](https://GitHub.com/stevedunn/vogen/issues/) [![GitHub issues-closed](https://img.shields.io/github/issues-closed/Naereen/StrapDown.js.svg)](https://GitHub.com/stevedunn/vogen/issues?q=is%3Aissue+is%3Aclosed)
[![Vogen stable version](https://badgen.net/nuget/v/vogen)](https://nuget.org/packages/vogen)

<p align="center">
  <img src="./assets/cavey.png">
</p>

# Vogen: cure your Primitive Obsession

## Overview

This is a source generator and code analyser that generates strongly typed **domain ideas**. You provide this:

```csharp
[ValueObject(typeof(int))]
public partial struct CustomerId {
    // optional
    private static Validation Validate(int value) => value > 0 
        ? Validation.Ok 
        : Validation.Invalid("Customer IDs must be a positive number.");
}
```

... and Vogen generates this:

```csharp
    public partial struct CustomerId : System.IEquatable<CustomerId> {
        private readonly int _value;

        public readonly int Value => _value;

        public CustomerId() {
            throw new Vogen.ValueObjectValidationException("Validation skipped by attempting to use the default constructor...");
        }

        private CustomerId(int value) => _value = value;

        public static CustomerId From(int value) {
            CustomerId instance = new CustomerId(value);
            return instance;
        }

        public readonly bool Equals(CustomerId other) ...
        public readonly bool Equals(int primitive) ...
        public readonly override bool Equals(object obj) ...
        public static bool operator ==(CustomerId left, CustomerId right) ...
        public static bool operator !=(CustomerId left, CustomerId right) ...
        public static bool operator ==(CustomerId left, int right) ...
        public static bool operator !=(CustomerId left, int right) ...
        public static bool operator ==(int left, CustomerId right) ...
        public static bool operator !=(int left, CustomerId right) ...

        public readonly override int GetHashCode() ...

        public readonly override string ToString() ...
    }
```

The code analyser helps you avoid accidentally creating invalid objects:

```csharp
[ValueObject(typeof(int))]
public partial struct CustomerId {
    // Vogen already generates this as a private constructor to that you can't use it:
    // error CS0111: Type 'CustomerId' already defines a member called 'CustomerId' with the same parameter type
    public CustomerId() { }

    // error VOG008: Cannot have user defined constructors, please use the From method for creation.
    public CustomerId(int value) { }
}
```

```csharp
// error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.
CustomerId c = default;

// error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.
var c2 = default(CustomerId);
```

The main goal of this project is to achieve **almost the same speed and memory performance as using primitives directly**.

___

## What is the repository?

This is a Nuget package that contains a **source generator** and a **code analyser**. The analyser disallows code that could evade validation, meaning your types have more safety from invalid data.

The source generator generates [Value Objects](https://wiki.c2.com/?ValueObject). Value Objects help combat Primitive Obsession by wrapping simple primitives such as `int`, `string`, `double` etc. in a strongly-typed type.

Primitive Obsession (AKA StringlyTyped) means being obsessed with primitives.  It is a Code Smell that degrades the quality of software.

> "*Primitive Obsession is using primitive data types to represent domain ideas*" [#](https://wiki.c2.com/?PrimitiveObsession)

Some examples:

* instead of `int age` - we'd have `Age age`. `Age` might have validation that it couldn't be negative
* instead of `string postcode` - we'd have `Postcode postcode`. `Postcode` might have validation on the format of the text

The source generator is opinionated. The opinions help ensure consistency. The opinions are:

* A Value Object (VO) is constructed via a factory method named `From`, e.g. `Age.From(12)`
* A VO is equatable (`Age.From(12) == Age.From(12)`)
* A VO, if validated, is validated with a static method named `Validate` that returns a `Validation` result
* Any validation that is not `Validation.Ok` results in a `ValueObjectValidationException` being thrown

It is common to represent domain ideas as primitives, but primitives might not be able to fully describe the domain idea.  To use Value Objects instead of primitives, we simply swap code like this:

```csharp
public class CustomerInfo {
    private int _id;
    public CustomerInfo(int id) => _id = id;
}
```

.. to this:

```csharp
public class CustomerInfo {
    private CustomerId _id;
    public CustomerInfo(CustomerId id) => _id = id;
}
```

## Tell me more about the Code Smell

There's a blog post [here](https://dunnhq.com/posts/2021/primitive-obsession/) that describes it, but to summarise:

> Primitive Obsession is being *obsessed* with the *seemingly* **convenient** way that primitives, such as `ints` and `strings`, allow us to represent domain objects and ideas.

It is **this**:

```csharp
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
public class Person {
    public Age Age { get; } = Age.Unspecified;
}
```

We can do that with an `Instance` attribute:

```csharp
  [ValueObject(typeof(int))]
  [Instance("Unspecified", -1)]
  public readonly partial struct Age {
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
public readonly partial struct Centigrade {
    public static Validation Validate(float value) =>
        value >= -273 ? Validation.Ok : Validation.Invalid("Cannot be colder than absolute zero");
}
```

## Benchmarking

### How do I run the benchmarks?

`dotnet run -c Release -- --job short --filter *`

### Common scenario - underlying type of int with validation

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
| UsingValueObjectAsStruct | 248.9 ns | 18.82 ns | 1.03 ns |  1.22 | 0.0181 |     - |     - |     304 B |


# FAQ

## Why can't I just use `public record struct CustomerId(int id);`?

That doesn't give you validation. To validate `id`, you can't use the shorthand syntax. So you'd need to do:

```csharp
public record struct CustomerId
{
    public CustomerId(int id) {
        if(id <=0) throw new Exception(...)
    }
}
```

You might also provide other constructors which might not validate the data, thereby allowing invalid data into your domain. Those other constructors might not throw exception, or might throw different exceptions.  One of the opinions in Vogen is that any invalid data given to a Value Object throws a `ValueObjectValidationException`.

You could also use `default(CustomerId)` to evade validation.  In Vogen, there are analysers that catch this and fail the build, e.g:

```csharp
// error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.
CustomerId c = default;

// error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.
var c2 = default(CustomerId);
```

## Can I serialize and deserialize them?

Yes, by referencing [Vogen.Serialization](https://www.nuget.org/packages/Vogen.Serialization).

## It seems like a lot of overhead; I can validate the value myself

You could, but to ensure consistency throughout your domain, you'd have to **validate everywhere**. And Shallow's Law says that that's not possible:

> ⚖️ **Shalloway's Law**
_"when N things need to change and N > 1, Shalloway will find at most N - 1 of these things."_

Concretely: _"When 5 things need to change, Shalloway will find at most, 4 of these things."_

## If my VO is a `struct`, can I stop the use of `CustomerId customerId = default(CustomerId);`?

Yes. The analyser generates a compilation error.

## If my VO is a `struct`, can I stop the use of `CustomerId customerId = new(CustomerId);`?

Yes. The analyser generates a compilation error.

## If my VO is a struct, can I have my own constructor?

No. The parameter-less constructor is generated automatically, and the constructor that takes the underlying value is also generated automatically.

If you add further constructors, then you will get a compilation error from the code generator, e.g.

```csharp
[ValueObject(typeof(int))]
public partial struct CustomerId {
    // Vogen already generates this as a private constructor:
    // error CS0111: Type 'CustomerId' already defines a member called 'CustomerId' with the same parameter type
    public CustomerId() { }

    // error VOG008: Cannot have user defined constructors, please use the From method for creation.
    public CustomerId(int value) { }
}
```

## If my VO is a struct, can I have my own fields?

You could, but you'd get compiler warning [CS0282-There is no defined ordering between fields in multiple declarations of partial class or struct 'type'](https://docs.microsoft.com/en-us/dotnet/csharp/misc/cs0282)

## Why are there no implicit conversions to and from the primitive types that are being wrapped?

Implicit operators can be useful, but for Value Objects, they can confuse things. Take the following code **without** any implicit conversions:

```csharp
Age age1 = Age.From(1);
OsVersion osVersion = OsVersion.From(1);

Console.WriteLine(age1 == osVersion); // won't compile! \o/
```

That makes perfect sense. But adding in an implicit operator **from** `Age` **to** `int`, and it does compile!

`Console.WriteLine(age1 == osVersion); // TRUE! (◎_◎;)`

If we remove that implicit operator and replace it with an implicit operator **from** `int` **to** `Age`, it no longer compiles, which is great (we've got type safety back), but we end up [violating the rules of implicit operators](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/user-defined-conversion-operators):

> Predefined C# implicit conversions always succeed and never throw an exception. User-defined implicit conversions should behave in that way as well. If a custom conversion can throw an exception or lose information, define it as an explicit conversion

In my research, I read some other opinions, and noted that the guidelines listed in [this answer](https://softwareengineering.stackexchange.com/a/284377/30906) say:

* If the conversion can throw an `InvalidCast` exception, then it shouldn't be implicit.
* If the conversion causes a heap allocation each time it is performed, then it shouldn't be implicit.

Which is interesting - Vogen _wouldn't_ throw an `InvalidCastException` (only an `ValueObjectValidationException`).  Also, for `struct`s, we _wouldn't_ create a heap allocation.

But since users of Vogen can declare a Value Object as a `class` **or** `struct`, then we wouldn't want implicit operators (from `primitive` => `ValueObject`) for just `structs` and not `class`es.

## Can you opt-in to implicit conversions?

No, but you can provide them yourself. For certain types it would allow a much more natural way of expressing, er, expressions.

Although it can be confusing. Let's say there's a type like this (and imagine that there's implicit conversions to `Age` and to `int`'):

```csharp
[ValueObject(typeof(int))]
public readonly partial struct Age
    public static Validation Validate(int n) => n >= 0 ? Validation.Ok : Validation.Invalid("Must be zero or more");
}
```

That says that `Age` instances can never be negative.  So you would probably expect the following to throw, but it doesn't:

```csharp
var age20 = Age.From(20);
var age10 = age20 / 2;
++age10;
age10 -= 12; // bang - goes negative??
```

But no..  The implicit cast in `var age10 = age20 / 2` results in an `int` and not an `Age`. Changing it to `Age age10 = age20 / 2` fixes it. But this does go to show that it can be confusing.

## Why is there no interface?

> _If I'm using a library that uses Vogen, I'd like to easily tell if the type is just a primitive wrapper or not by the fact that it implements an interface, such as `IValidated<T>`_

Just like primitives have no interfaces, there's no need to have interfaces on Value Objects. The receiver that takes a `CustomerId` knows that it's a Value Object.  If it were instead to take an `IValidated<int>`, then it wouldn't have any more information; you'd still have to know to call `Value` to get the value.

It might also relax type safety. Without the interface, we'd have signatures such as this:

```csharp
public void SomSomething(CustomerId customerId, SupplierId supplierId, ProductId productId);
```

... but with the interface, we _could_ have signatures such as this:

```csharp
public void SomSomething(IValidate<int> customerId, IValidated<int> supplierId, IValidated<int> productId);
```

So, callers could mess things up by calling `DoSomething(productId, supplierId, customerId)`)

There would also be no need to know if it's validated, as, if it's in your domain, it's valid (there's no way to manually create invalid instances).  And with that said, there'd also be no point in exposing the 'Validate' method via the interface because validation is done at creation.

## Why are they called 'Value Objects'?
The term Value Object represents a small object whos equality is based on value and not identity. From [Wikipedia](https://en.wikipedia.org/wiki/Value_object)

> _In computer science, a value object is a small object that represents a simple entity whose equality is not based on identity: i.e. two value objects are equal when they have the same value, not necessarily being the same object._

In DDD, a Value Object is (again, from [Wikipedia](https://en.wikipedia.org/wiki/Domain-driven_design#Building_blocks))

>  _... a value object is an immutable object that contains attributes but has no conceptual identity_

## Can I represent special values

Yes. You might want to represent special values for things like invalid or unspecified instances, e.g.

```csharp
/*
* Instances are the only way to avoid validation, so we can create instances
* that nobody else can. This is useful for creating special instances
* that represent concepts such as 'invalid' and 'unspecified'.
*/
[ValueObject(typeof(int))]
[Instance("Unspecified", -1)]
[Instance("Invalid", -2)]
public readonly partial struct Age
{
    private static Validation Validate(int value) =>
        value > 0 ? Validation.Ok : Validation.Invalid("Must be greater than zero.");
}
```

You can then use default values when using these types, e.g.

```csharp
public class Person {
    public Age Age { get; set; } = Age.Unspecified
}
```

... and if you take an Age, you can compare it to an instance that is invalid/unspecified

```csharp
public void CanEnter(Age age) {
    if(age == Age.Unspecified || age == Age.Invalid) throw CannotEnterException("Name not specified or is invalid")
    return age < 17;
}
```

# What alternatives are there?

[ValueOf](https://github.com/mcintyre321/ValueOf) 

[StringlyTyped](https://github.com/stevedunn/stringlytyped)

[ValueObjectGenerator](https://github.com/RyotaMurohoshi/ValueObjectGenerator)