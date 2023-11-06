# Overview

<chapter>
Vogen is a source generator. The source generator generates strongly typed **domain concepts**. 
</chapter>

You provide this:

``` c#
[ValueObject<int>]
public partial struct CustomerId {
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


You then use `CustomerId` instead of `int` in your domain in the full knowledge that it is valid and safe to use:

```c#
CustomerId customerId = CustomerId.From(123);
SendInvoice(customerId);
...

public void SendInvoice(CustomerId customerId) { ... }
```

The main goal of Vogen is to **ensure the validity of your Value Objects**, the code analyzer helps you to avoid mistakes which
might leave you with uninitialized Value Objects in your domain.

It does this by **adding new constraints in the form of new C# compilation errors**. There are a few ways you could end up
with uninitialized Value Objects. One way is by giving your type constructors. Providing your own constructors
could mean that you forget to set a value, so **Vogen doesn't allow you to have user defined constructors**:

```c#
[ValueObject]
public partial struct CustomerId {
    // Vogen deliberately generates this so that you can't create your own:
    // error CS0111: Type 'CustomerId' already defines a member called 'CustomerId' with the same parameter type
    public CustomerId() { }

    // error VOG008: Cannot have user defined constructors, please use the From method for creation.
    public CustomerId(int value) { }
}
```

In addition, Vogen will spot issues when **creating** or **consuming** Value Objects:

```c#
// catches object creation expressions
var c = new CustomerId(); // error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited
CustomerId c = default; // error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.

var c = default(CustomerId); // error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.
var c = GetCustomerId(); // error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited

var c = Activator.CreateInstance<CustomerId>(); // error VOG025: Type 'CustomerId' cannot be constructed via Reflection as it is prohibited.
var c = Activator.CreateInstance(typeof(CustomerId)); // error VOG025: Type 'MyVo' cannot be constructed via Reflection as it is prohibited

// catches lambda expressions
Func<CustomerId> f = () => default; // error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.

// catches method / local function return expressions
CustomerId GetCustomerId() => default; // error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.
CustomerId GetCustomerId() => new CustomerId(); // error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited
CustomerId GetCustomerId() => new(); // error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited

// catches argument / parameter expressions
Task<CustomerId> t = Task.FromResult<CustomerId>(new()); // error VOG010: Type 'CustomerId' cannot be constructed with 'new' as it is prohibited

void Process(CustomerId customerId = default) { } // error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.
```

One of the main goals of this project is to achieve **almost the same speed and memory performance as using primitives directly**.
Put another way, if your `decimal` primitive represents an Account Balance, then there is **extremely** low overhead of
using an `AccountBalance` Value Object instead. Please see the [performance metrics below](#performance).
