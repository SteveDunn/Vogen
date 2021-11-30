This is a semi-opinionated library which generates [Value Objects](https://wiki.c2.com/?ValueObject) that wrap simple primitives such as `int`, `string`, `double` etc. The main goal of this project is to have almost the same speed and memory performance as using primitives.

A ValueObject is a strongly typed (strongly, not stringly) domain object that is immutable.

Instead of
```csharp
int customerId = 42;
``` 

we have

``` cs
var customerId = CustomerId.From(42);
```

CustomerId derives from this package's ValueObject type:

```csharp
[ValueObject(typeof(int))]
public partial struct CustomerId
{
}
```

Here it is again with some validation

``` cs
public class CustomerId : ValueObject<int, CustomerId>
{
    private static Validation Validate(int value) => 
        value > 0 ? Validation.Ok : Validation.Invalid(); }
```

This allows us to have more strongly typed domain objects instead of primitives, which makes the code easier to read and enforces better method signatures, so instead of:

```csharp
public void DoSomething(int customerId, int supplierId, int amount)
```

we can have:

```csharp
public void DoSomething(CustomerId customerId, SupplierId supplierId, Amount amount)
```

Now, callers can't mess up the ordering of parameters and accidentally pass us a Supplier ID in place of a Customer ID.

It also means that validation is in just one place. You can't introduce bad objects into your domain, therefore you can assume that, in your domain, every ValueObject is valid. Handy.