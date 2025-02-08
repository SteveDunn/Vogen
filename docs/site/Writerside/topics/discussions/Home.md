# Primitive Obsession

<tldr>
TL;DR: Primitive Obsession is being obsessed with the seemingly convenient way that primitives, such as integers 
and strings, allow us to represent domain objects and ideas. Although convenient, they come with a hefty cost... 
</tldr>

Primitive Obsession (AKA StringlyTyped) means being obsessed with primitives.  It is a Code Smell that degrades the 
quality of software.

There's a blog post [here](https://dunnhq.com/posts/2021/primitive-obsession/) that describes it more fully;
what follows is a summary.

Primitive Obsession is **this**:

```c#
int customerId = 42
```

What's wrong with that?

An `int` likely cannot *fully* represent a customer ID.  An `int` can be negative or zero, but it's unlikely a customer
ID can be. So, we have **constraints** on a customer ID.  We can't _represent_ or _enforce_ those constraints on an `int`.

So, we need some validation to ensure the **constraints** of a customer ID are met. Because it's in `int`, we can't be
sure if it's been checked beforehand, so we need to check it every time we use it.  Because it's a primitive, 
someone might've changed the value, so even if we're 100% sure we've checked it before, it still might need checking again.

So far, we've used as an example, a customer ID of value 42. In C#, it may come as no surprise that this 
is true: "`42 == 42`" (*I haven't checked that in JavaScript!*). But in our **domain**, should 42 always 
equal 42? Probably not if you're comparing a **Supplier ID** of 42 to a **Customer ID** of 42! But primitives 
won't help you here (remember, `42 == 42`!):

```c#
(42 == 42) // true
(SupplierId.From(42) == SupplierId.From(42)) // true
(SupplierId.From(42) == VendorId.From(42)) // compilation error
```

But sometimes, we need to denote that a Value Object isn't valid or has not been set. We don't want anyone _outside_ of the object doing this as it could be used accidentally.  It's common to have `Unspecified` instances, e.g.

```c#
public class Person {
    public Age Age { get; } = Age.Unspecified;
}
```

We can do that with _Instances_, which is covered more in [this tutorial](Specifying-pre-set-values.md).
Instances allows `new` to be used _in the value object itself_, which bypasses validation and normalization. More information on validation can be found in [this tutorial](ValidationTutorial.md). 

```c#
  [ValueObject]
  public readonly partial struct Age {
      public static readonly Age Unspecified = new(-1);

      public static Validation Validate(int value) =>
          value > 0 
            ? Validation.Ok 
            : Validation.Invalid("Must be greater than zero.");
  }
```

The constructor is `private`, so only this type can (deliberately) create _invalid_ instances.

Now, when we use `Age`, our validation becomes clearer:

```c#
public void Process(Person person) {
    if(person.Age == Age.Unspecified) {
        // age not specified.
    }
}
```

Primitive Obsession can also help introduce bugs into your software. Take, for instance, the following method:

```c#
public void IncreaseQuantity(
    int customerId, int supplierId, int quantity) 
```

… and a caller calls it like this:

```C#
_service.IncreaseQuantity(_supplierId, _customerId, _quantity)
```

We've messed up the order of the parameters, but our compiler won't tell us.
The best we can hope for is a failing unit test. 
However, given the contrived data often used in unit tests, it could be that the data will hide the problem by using 
the same ID for customer and supplier.

With Value Objects representing `SupplierId`, `CustomerId`, and `Quantity`, the compiler **can** tell us
if we mess up the order. These types make the domain code more understandable (more domain language and less C#
types), and validation is as close to the data as possible; in this example, it likely means that all these
types cannot be zero or negative.
