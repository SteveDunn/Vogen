# Specifying pre-set values

In this tutorial, we'll look at how we can have pre-set values on our types.

Pre-set values have two common uses:

1. to represent known values
2. to represent values that _users_ of a Value Object can’t create

Let's look at the first scenario, representing known values. Create the following type:

```c#
[ValueObject<float>]
public partial struct Centigrade
{
   public static readonly Centigrade WaterFreezingPoint = From(0.0f);
   public static readonly Centigrade WaterBoilingPoint = From(100.0f);
   public static readonly Centigrade AbsoluteZero = From(-273.15f);
}
```

You can now use it like so:

```C#
Console.WriteLine(Centigrade.WaterFreezingPoint);
Console.WriteLine(Centigrade.WaterBoilingPoint);
Console.WriteLine(Centigrade.AbsoluteZero);
```

... resulting in

```
0
100
-273.15
```

These known instances can bring domain terms into your code; for instance, it's easier to read this than 
numeric literals of `0` and `-273.15`:

```C#
if(waterTemperature == Centigrade.WaterFreezingPoint) ...
```

Now, let's take a look at the other scenario of representing values that can't (and **shouldn't**) be
created externally. The term 'externally' user here, means **users** of the class.

Let's revisit our `CustomerId` from the [validation tutorial](ValidationTutorial.md). We want to say that an instance
with a value of zero means that the customer wasn’t specified, but we don't want users to explicitly create
instances with a value of zero. Let's try it out. Create this type again:  

```C#
[ValueObject<int>]
public partial struct CustomerId
{
    private static Validation Validate(int input) => input > 0 
        ? Validation.Ok 
        : Validation.Invalid("Customer IDs must be greater than 0.");    
}
```

We know from the validation tutorial the above code throws an exception.
This means that users can't create one with a zero value. All well and good. But **we** (the author of the type), want to create one with a zero.

We can do this with a known-instance:

```C# 
[ValueObject<int>]
public partial struct CustomerId
{
    public static readonly CustomerId Unspecified = new(0);
    
    private static Validation Validate(int input) => input > 0 
        ? Validation.Ok 
        : Validation.Invalid("Customer IDs must be greater than 0.");    
}
```

We can now use the instance of an unspecified customer id:

```C#
Console.WriteLine(CustomerId.Unspecified);

>> 0
```

This can be useful in representing optional or missing data in your domain, e.g.

```C#
public CustomerId TryGetOptionalCustomerId(string input)
{
    if (string.IsNullOrEmpty(input))
    {
        return CustomerId.Unspecified;
    }

    return CustomerId.From(123);
}
```

This again makes the domain easier to read and eliminates a scenario where a `null` might otherwise be used. 

There were other ways to declare instances which are now obsolete, as described in 
this [How-to article](Instances.md)
