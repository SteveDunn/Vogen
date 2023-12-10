# Your first Value Object

<card-summary>
Create and use your first Value Object
</card-summary>

In this tutorial, we'll create and use a Value Object.

[Install](Installation.md) the package, and then, in your project, create a new Value Object that represents a Customer ID:

```C#
[ValueObject<int>] 
public partial struct CustomerId { }
```

If you're not using generics, you can use `typeof` instead:

```c#
[ValueObject(typeof(int))] 
public partial struct CustomerId { }
```

<note>
the partial keyword is required as the code generator augments this type by creating another partial class
</note>

Now, create a new instance by using the `From` method that the source generator generates:

```c#
var customerId = CustomerId.From(42);
```

If you try to use a constructor, then the [analyzer rules](Analyzer-Rules.md) will catch this and stop you.

You can now be more explicit in your methods with signatures such as:

```c#
public void HandlePayment(
    CustomerId customerId, 
    AccountId accountId, 
    PaymentAmount paymentAmount)
```


