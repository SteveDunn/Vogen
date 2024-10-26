# Vogen - Value Object Generator

# What is the package?

This is a _semi_-opinionated library which is a Source Generator to generate [value objects](https://wiki.c2.com/?ValueObject).
The main goal is that the value objects generated have almost the same speed and memory performance as using primitives.

The value objects wrap simple primitives such as `int`, `string`, `double` etc.

To get started, add this package, and add a type such as:

```csharp
[ValueObject<int>]
public partial struct CustomerId 
{
}
```

You can now treat `CustomerId` as you would an `int` and there is very little performance difference between the two:

`var id = CustomerId.From(42);`

And your method signatures change from:

```charp
void HandleCustomer(int customerId)
```

to

```charp
void HandleCustomer(CustomerId customerId)
```

The Source Generator generates code for things like creating the object and for performing equality.

Value objects help combat Primitive Obsession. Primitive Obsession means being obsessed with primitives.  It is a Code Smell that degrades the quality of software.

> "*Primitive Obsession is using primitive data types to represent domain ideas*" [#](https://wiki.c2.com/?PrimitiveObsession)

Some examples:

* instead of `int age` - we'd have `Age age`. `Age` might have validation that it couldn't be negative
* instead of `string postcode` - we'd have `Postcode postcode`. `Postcode` might have validation on the format of the text

The opinions are expressed as:

* A value object (VO) is constructed via a factory method named `From`, e.g. `Age.From(12)`
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
[ValueObject<int>]
public partial struct CustomerId 
{
}
```
That's all you need to do to switch from a primitive to a value object.

Here it is again with some validation

```csharp
[ValueObject<int>]
public partial struct CustomerId 
{
    private static Validation Validate(int value) => 
        value > 0 ? Validation.Ok : Validation.Invalid(); 
}
```

This allows us to have more _strongly typed_ domain objects instead of primitives, which makes the code easier to read and enforces better method signatures, so instead of:

``` cs
void DoSomething(int customerId, int supplierId, int amount)
```
we can have:

``` cs
void DoSomething(CustomerId customerId, SupplierId supplierId, Amount amount)
```

Now, callers can't mess up the ordering of parameters and accidentally pass us a Supplier ID in place of a Customer ID.

It also means that validation **is in just one place**. You can't introduce bad objects into your domain, therefore you can assume that **in _your_ domain** every ValueObject is valid.

## Adding the package

Add the package to your application using

```bash
dotnet add package Vogen
```

This adds a `<PackageReference>` to your project. You can additionally mark the package as `PrivateAssets="all"` and `ExcludeAssets="runtime"`.

> Setting `PrivateAssets="all"` means any projects referencing this one won't get a reference to the _Vogen_ package. Setting `ExcludeAssets="runtime"` ensures the _Vogen.SharedTypes.dll_ file is not copied to your build output (it is not required at runtime).

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <!-- Add the package -->
  <PackageReference Include="Vogen" Version="4.0.0" 
    PrivateAssets="all" ExcludeAssets="runtime" />
  <!-- -->

</Project>
```

## How does it compare to using native types?

Here's the benchmarks comparing a native int to a ValueObject:

|                  Method |     Mean |    Error |   StdDev | Ratio | Allocated |
|------------------------ |---------:|---------:|---------:|------:|----------:|
|        UsingIntNatively | 17.04 ns | 0.253 ns | 0.014 ns |  1.00 |         - |
|  UsingValueObjectStruct | 19.76 ns | 2.463 ns | 0.135 ns |  1.16 |         - |

There's hardly any speed overhead, and no memory overhead.

The next most common scenario is using a VO to represent a string:

|                   Method |     Mean |    Error |  StdDev | Ratio | Allocated |
|------------------------- |---------:|---------:|--------:|------:|----------:|
|      UsingStringNatively | 204.4 ns |  8.09 ns | 0.44 ns |  1.00 |     256 B |
|  UsingValueObjectAsClass | 250.7 ns | 29.97 ns | 1.64 ns |  1.23 |     328 B |
| UsingValueObjectAsStruct | 248.9 ns | 18.82 ns | 1.03 ns |  1.22 |     304 B |
