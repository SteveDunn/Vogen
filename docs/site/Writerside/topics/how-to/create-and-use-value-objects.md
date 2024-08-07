# Create and use Value Objects

<card-summary>
How to create and use Value Objects, how they better represent domain concepts, and how they help ensure
that no invalid instances can be created.
</card-summary>

Create a new value object by decorating a partial type with the `ValueObject` attribute:

```c#
[ValueObject<int>] 
public partial struct CustomerId { }
```

The type must be `partial` as the source generator augments the type with another partial class containing the
generated code.

Now, create a new instance by using the `From` method:

```c#
var customerId = CustomerId.From(42);
```

If you try to use the constructors, the [analyzer rules](Analyzer-Rules.md) will catch this and stop you.

You can now be more explicit in your methods with signatures such as:

```c#
public void HandlePayment(
    CustomerId customerId, 
    AccountId accountId, 
    PaymentAmount paymentAmount)
```

If your type has [validation](Validate-values.md), then the `From` method will throw an exception if validation fails.

This stops invalid instances being created, but exceptions might not be desirable. 
In this case, use the `TryFrom` method.
There are two overloads for this. 
One returns a `ValueObjectOrError<T>` and one returns a bool and takes an `out` parameter.

