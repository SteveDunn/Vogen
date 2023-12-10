# Validating user-provided values

<card-summary>
Validate user-provided values
</card-summary>

In this tutorial, we're going to extend the `CustomerId` type we created in 
the [first tutorial](your-first-value-object.md).

Vogen calls the `Validate` method when you create your types. Let's stop users from creating Customer IDs with
values of zero or less.

Add the following `Validate` method to `CustomerId`:

```C#
private static Validation Validate(int input) => input > 0 
    ? Validation.Ok 
    : Validation.Invalid("Customer IDs must be greater than 0.");
```

Now, when you try to create one:

```C#
var newCustomer = CustomerId.From(0);
```

you'll get an exception at runtime:
```
Unhandled exception. Vogen.ValueObjectValidationException: 
    Customer IDs must be greater than 0.
```

Note that the method is `private` and `static`.
The method doesn't need to be `public` because it doesn't make sense for
anyone to call it; if something has a `CustomerId`, then **it is valid**, so users don't need to check the validity.
It is also `static` because it's not related to any particular _instance_ of this type.

For more advanced scenarios, please see the [How-to article on validation](Validate-values.md)
