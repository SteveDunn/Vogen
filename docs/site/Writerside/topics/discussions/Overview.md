# Overview

<p >
<img alt="cavey-logo.png" src="cavey-logo.png" width="296"/>
</p>

[Vogen](https://github.com/SteveDunn/Vogen) wraps .NET primitives. You provide this:

``` c#
[ValueObject<int>]
public partial struct CustomerId 
{
}
```

... and Vogen generates source code similar to this:

```c#
public partial struct CustomerId : System.IEquatable<CustomerId>, 
    System.IComparable<CustomerId>, 
    System.IComparable 
{
    private readonly int _value;

    public readonly int Value => _value;

    public CustomerId() {
        throw new ValueObjectValidationException("Validation skipped");
    }

    private CustomerId(int value) => _value = value;

    public static CustomerId From(int value) {
        CustomerId instance = new CustomerId(value);
        return instance;
    }

    public readonly bool Equals(CustomerId other) ...
    public readonly bool Equals(int primitive) ...
    public readonly override bool Equals(object obj) ...
    public static bool operator ==(CustomerId left, CustomerId right)
    public static bool operator !=(CustomerId left, CustomerId right)
    public static bool operator ==(CustomerId left, int right) ...
    public static bool operator !=(CustomerId left, int right) ...
    public static bool operator ==(int left, CustomerId right) ...
    public static bool operator !=(int left, CustomerId right) ...

    public readonly override int GetHashCode() ...

    public readonly override string ToString() ...
}
```

Now, in your **domain**, you can use `CustomerId` instead of `int`
with the confidence that it is valid and represents _exactly_ what it says:

```c#
CustomerId customerId = CustomerId.From(123);
SendInvoice(customerId);
...

public void SendInvoice(CustomerId customerId) { ... }
```

The main goal of Vogen is to **ensure the validity of your Value Objects**.

It does this with code analyzers that add constraints to C#.
The analyzers spot situations where you might end up with uninitialized Value Objects.
These analyzers, by default, cause compilation errors.

An example violation is where you attempt to add a constructor to your Value Object.
In the types tha that Vogen creates, there is one, and only one, way of creating 
instances, and that is with generated `From` method. 
Relying on user-provided constructors could mean that you forget to set or validate the value.
Vogen spots when you create your own constructors and **causes a compilation error**:

```c#
[ValueObject]
public partial struct CustomerId 
{
    // error CS0111: Type 'CustomerId' already defines a member called 
    // 'CustomerId' with the same parameter type
    // That's because Vogen deliberately generates this 
    // so that you can't create your own.
    public CustomerId() { }

    // error VOG008: Cannot have user defined constructors, please use 
    // the From method for creation.
    public CustomerId(int value) { }
}
```

Here's some of the issues that Vogen will spot when **creating** or **consuming** Value Objects:

```c#
// --- catches object creation expressions

// error VOG010: Type 'CustomerId' cannot be constructed 
// with 'new' as it is prohibited
var c = new CustomerId(); 

// error VOG009: ... cannot be constructed with default ...
CustomerId c = default; 

// error VOG009: ... cannot be constructed with default ...
var c = default(CustomerId); 


// error VOG010: ... cannot be constructed with 'new' ...
var c = GetCustomerId(); 

// error VOG025: Type 'CustomerId' cannot be constructed via Reflection
var c = Activator.CreateInstance<CustomerId>(); 

// error VOG025: Type 'MyVo' cannot be constructed via Reflection
var c = Activator.CreateInstance(typeof(CustomerId)); 

// catches lambda expressions, e.g.
// error VOG009: Type 'CustomerId' cannot be constructed with default
Func<CustomerId> f = () => default; 

// --- catches method / local function return expressions, e.g.

// error VOG009: Type 'CustomerId' cannot be constructed with default
CustomerId GetCustomerId() => default; 

// error VOG010: Type 'CustomerId' cannot be constructed with 'new'
CustomerId GetCustomerId() => new CustomerId(); 

// error VOG010: Type 'CustomerId' cannot be constructed with 'new'
CustomerId GetCustomerId() => new();

// --- catches argument / parameter expressions

// error VOG010: Type 'CustomerId' cannot be constructed with 'new'
Task<CustomerId> t = Task.FromResult<CustomerId>(new()); 

// error VOG009: Type 'CustomerId' cannot be constructed with default
void Process(CustomerId customerId = default) { } 
```

One of the main goals of Vogen is to achieve **almost the same speed and memory performance as using
primitives directly**.
Put another way, if your `decimal` primitive represents an Account Balance, then there 
is **extremely** low overhead of using an `AccountBalance` Value Object instead. 
Please see the [performance metrics](Performance.md) for more information.


## How this documentation is organized

* These top level pages discuss key topics and concepts at a fairly high level and provide useful background information
  and explanation.

* Tutorials—take you by the hand through a series of steps to create an application that uses Vogen.
  Start here if you’re new to Vogen.

* Reference guides-contains technical reference for Vogen usage, describing how it works and how to use it.
  The Assumption is that you have a basic understanding of key concepts.

* How-to guides—recipes guiding you through the steps involved in addressing key problems and use-cases.
  They are more advanced than tutorials, and they assume some knowledge of how Vogen works.


## Attribution

I took inspiration from [Andrew Lock's](https://github.com/andrewlock) [StronglyTypedId](https://github.com/andrewlock/StronglyTypedId) and got some great ideas 
from [Gérald Barré's](https://github.com/meziantou) [Meziantou.Analyzer](https://github.com/meziantou/Meziantou.Analyzer)
