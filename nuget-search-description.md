This package contains a very simple implementation of a ValueObject in .NET.

A ValueObject is a strongly typed (strongly, not stringly) domain object that is immutable.

Instead of
``` cs
int customerId = 42;
``` we have

``` cs
var customerId = CustomerId.From(42);
```

CustomerId derives from this package's ValueObject type:

``` cs
public class CustomerId : ValueObject<int, CustomerId>
{
}
```

Here it is again with some validation

``` cs
public class CustomerId : ValueObject<int, CustomerId>
{
    public override Validation Validate() => Value > 0 
      ? Validation.Ok 
      : Validation.Invalid("Customer IDs cannot be zero or negative.");
}
```
This allows us to have more strongly typed domain objects instead of primitives, which makes the code easier to read and enforces better method signatures, so instead of:

``` cs
public void DoSomething(int customerId, int supplierId, int amount)
```
we can have:

``` cs
public void DoSomething(CustomerId customerId, SupplierId supplierId, Amount amount)

Now, callers can't mess up the ordering or parameters and accidentally pass us a Supplier ID in place of a Customer ID.

It also means that validation is in just one place. You can't introduce bad objects into your domain, therefore you can assume that in your domain every ValueObject is valid. Handy.