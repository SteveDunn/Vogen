# Normalizing user-provided values

In this tutorial, we'll see how to normalize (clean-up) input when creating value objects.

We'll create a type that represents a vehicle registration plate in the UK. Vehicle registration plates vary from
country to country, but in the UK, they are all upper case, e.g. `AB12 XYZ`.

Create a `VehicleRegistrationPlate` type:


```C#
[ValueObject<string>] 
public partial struct VehicleRegistrationPlate 
{
}
```

By default, creating one and using it does nothing with the value provided:

```C#
var plate = VehicleRegistrationPlate.From("ab12 xyz");
Console.WriteLine(plate);

>> ab12 xyz
```

Now, add the following method:

```C#
private static string NormalizeInput(string input) => 
    input.ToUpperInvariant();
```

... and run it again to see that the data has been normalized as upper-case.

```C#
var plate = VehicleRegistrationPlate.From("ab12 xyz");
Console.WriteLine(plate);

>> AB12 XYZ
```

Note that normalization happens before validation, so your method might get passed invalid raw data.

Also note that if you [validate your values](ValidationTutorial.md), then validation happens on the sanitized value.
