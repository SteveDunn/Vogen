This is a sample web application. It shows how to use value objects
in a web app.

In particular, it shows how to get Vogen to generate the necessary code to
customize the OpenAPI schema types for value object (e.g. `CustomerName` is a `string`, `Celcius` is a `number` etc.)

The project can also be switched to use `Microsof.OpenApi` and 'Scalar'. This targets .NET9. This is merely here as 
a placeholder. It was added to see if there was a way, similiar to Swashbuckle, to customize the schema types of 
value objects. I haven't found a way to do that yet though as it's not as customizable as Swashbuckle.
You can switch by changing `<OpenApiMode>` in the `.csproj` file to `MicrosoftAndScalar`, `Swashbuckle-net8`,
or `Swashbuckle-net10`. 
The launch settings for `MicrosoftAndScalar` is `https openapi and scalar`.

The companion project to this is `WebApplicationConsumer` which demonstrates how to consume an API that uses value 
object as parameters.