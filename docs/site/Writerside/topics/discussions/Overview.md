# Overview

Vogen wraps .NET primitives. You provide this:

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


Then, in your **domain**, you use `CustomerId` instead of `int`
with the confidence that it is valid and represents _exactly_ what it says:

```c#
CustomerId customerId = CustomerId.From(123);
SendInvoice(customerId);
...

public void SendInvoice(CustomerId customerId) { ... }
```

The main goal of Vogen is to **ensure the validity of your Value Objects**.

It does this with code analyzers that add constraints to C#.
The analyzers spot situations where you might end up with uninitialized Value Objects in your domain.
These analyzers, by default, cause compilation errors.

There are a few ways you can end up with uninitialized Value Objects. 
One way is by giving your type constructors. Providing your own constructors could mean that you 
forget to set a value, so **Vogen doesn't allow user defined constructors**:

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

In addition, Vogen will spot issues when **creating** or **consuming** Value Objects:

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

## Attribution

I took inspiration from [Andrew Lock's](https://github.com/andrewlock) [StronglyTypedId](https://github.com/andrewlock/StronglyTypedId) and got some great ideas 
from [Gérald Barré's](https://github.com/meziantou) [Meziantou.Analyzer](https://github.com/meziantou/Meziantou.Analyzer)
