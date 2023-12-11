# Working with JSON

<note>
This topic is incomplete and is currently being improved.
</note>

As well as System.Text.Json (STJ), Vogen also generates the code to work with Newtonsoft.Json (NSJ)

They are controlled by the `Conversions` enum. The following has serializers for NSJ and STJ:

```c#
[ValueObject<float>(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson)]
public readonly partial struct Celsius { }
```

See the examples folder for more information.
