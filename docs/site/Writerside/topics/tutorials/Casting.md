# Cast to and from the underlying type

This page describes casting to and from the underlying primitive, using either explicit or implicit casting operators. However, it is **not** recommended to use implicit casting. We'll first describe how to use them and then describe why the problems with implicit casts. 

To generate casts, set one of the `...PrimitiveCasting` flags in the constructor of the `ValueObject` attribute. 

You can specify casting **to** the primitive, or **from** the primitive. Here's an example of implicitly casting to and from the primitive.

<note>
C# allows only one casting operator, be that implicit or explicit, but not both
</note>

Vogen defaults to generating explicit casts both ways (to and from the primitive), which means you can do things like this:

```c#
Age age1 = Age.From(42);

int n = (int)age1;
Age age2 = (Age)42;
```

Explicitly casting from the primitive (`var x= (Age)42`) is the same as calling `Age.From(42); any validation and normalization methods are called.

You _could_ also specify implicit casting:

```c#
[ValueObject<int>(
  fromPrimitiveCasting: CastOperator.Implicit, 
  toPrimitiveCasting: CastOperator.Implicit)]
public partial class Age;
```

This allows you to do things like:

```c#
int age1 = Age.From(42);

int n = age1;
Age a2 = 42;
```

Implicit casting bypasses any validation and normalization methods.

<note>
An implicit cast does not throw an exception—this is described more fully below
</note>

That's how to use both of them, so why shouldn't you use implicit casts?

## Why shouldn't I use them?

Using implicit casts might seem like a handy way to use the underlying primitive natively, but the goal of strong-typing primitives is to differentiate them from the underlying type.

Take, for instance, a `Score` type:

```c#
[ValueObject<int>(
  fromPrimitiveCasting: CastOperator.Implicit, 
  toPrimitiveCasting: CastOperator.Implicit)]
public partial struct Score;
```

An implicit cast would allow easy access to the `Value`, allowing things such as
`int n = _score + 10;`. 

But what would be preferable is to be more explicit and add a method that describes the operation, something like:

```c#
[ValueObject<int>]
public partial struct Score 
{ 
    public Score IncreaseBy(Points points) => 
        From(_value + points.Value);
}
```

This is more explicit. It says that scores can be *increased* but not *decreased* (of course, `Points` could allow negatives, but that's up to you and your domain).

There's no denying that implicit operators can be useful. But as useful as they are, they can confuse things and potentially introduce errors, for example:

```c#
[ValueObject(
  fromPrimitiveCasting: CastOperator.Implicit, 
  toPrimitiveCasting: CastOperator.Implicit)]
public partial class Age;

[ValueObject(
  fromPrimitiveCasting: CastOperator.Implicit, 
  toPrimitiveCasting: CastOperator.Implicit)]
public readonly partial struct OsVersion;

Age age = Age.From(1);
int anInt = age;
Print(anInt);

void Print(OsVersion osVersion) => 
    Console.WriteLine(osVersion);
```

We can see that we lose type-safety. An `Age` is implicitly an int, and an `OsVersion` is implicitly an int. Therefore, the call to `Print`, which takes an `OsVersion`, also, implicitly, takes an `int`!

There are also issues with validation that [violates the rules of implicit operators](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/user-defined-conversion-operators), which is:

> Predefined C# implicit conversions always succeed and never throw an exception. User-defined implicit conversions should behave in that way as well. If a custom conversion can throw an exception or lose information, define it as an explicit conversion

But a primitive cast to a value object might not always succeed due to validation.

Other opinions, such as the guidelines listed in [this answer](https://softwareengineering.stackexchange.com/a/284377/30906) say:

* If the conversion can throw an `InvalidCastException`, then it shouldn't be implicit
* If the conversion causes a heap allocation each time it is performed, then it shouldn't be implicit

If Vogen did throw during an implicit cast, it _wouldn't_ throw an `InvalidCastException` (only an `ValueObjectValidationException`).  Also, for classes, it _would_ create a heap allocation (but value objects can be a `class` or `struct`, so it wouldn't make sense to allow them one and not the other).

As mentioned previously, validation is skipped for implicit casts (because they should never throw an exception).
Let's say there's a type like this: 

```c#
[ValueObject<int>(
  fromPrimitiveCasting: CastOperator.Implicit, 
  toPrimitiveCasting: CastOperator.Implicit)]
public readonly partial struct Age 
{
    public static Validation Validate(int n) => n >= 0 
      ? Validation.Ok 
      : Validation.Invalid("Must be zero or more");
}
```

That says that `Age` instances can never be negative, but primitive casting gets around this:

```c#
var age20 = Age.From(20);
var age10 = age20 / 2;
++age10;
age10 -= 12; // bang - goes negative??
```

The implicit cast in `var age10 = age20 / 2` results in an `int` and not an `Age`. Changing it to `Age age10 = age20 / 2` fixes it. But this does go to show that it can be confusing.

For the reasons above, it is recommended not to use implicit casts. When the urge to use them arises, it is worth thinking what _explicit concept_ is missing from the code, for example, `Score.IncreaseBy`. 
