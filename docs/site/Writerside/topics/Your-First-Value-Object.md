# Your First Value Object

After you've installed the package, create a new Value Object.

```c#
[ValueObject<int>] 
public partial struct CustomerId { }
```

Set the value with the `From` method instead of the constructor:

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


