# Validating user-provided values

<card-summary>
Validate user-provided values
</card-summary>

In this tutorial, we're going to extend the `CustomerId` type we created in 
the [first tutorial](your-first-value-object.md).

When Vogen generates the backing code for your type, it will call your `Validate` method if it exists.

Let's use that to stop the code creating Customer IDs with values of zero or less.

Add the following `Validate` method to `CustomerId`:

```C#
private static Validation Validate(int input) => input > 0 
    ? Validation.Ok 
    : Validation.Invalid("Customer IDs must be greater than 0.");
```

Add the following to try to create one:

```C#
var newCustomer = CustomerId.From(0);
```

You'll get an exception at runtime:
```
Unhandled exception. Vogen.ValueObjectValidationException: 
    Customer IDs must be greater than 0.
```

Note that the `Validate` method is `private` and `static`.
The method doesn't need to be `public` because it doesn't make sense for
anyone to call it; if something has a `CustomerId`, then **it is valid**, so users don't need to check the validity.
It is also `static` because it's not related to any particular _instance_ of this type.

<note>
Sometimes you need to represent special instance of a Value Object that represent concepts such as 'unspecified'.
Vogen allows pre-set instances to be declared on the type itself. 
Please refer to the <a href="Specifying-pre-set-values.md">tutorial on pre-set values</a>. 
</note>

For more advanced scenarios, please see the [How-to article on validation](Validate-values.md)
