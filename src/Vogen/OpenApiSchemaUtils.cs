namespace Vogen;

internal static class OpenApiSchemaUtils
{
    private static bool IsOpenApi1xReferenced(VogenKnownSymbols vogenKnownSymbols) => vogenKnownSymbols.OpenApiOptions is not null;
    private static bool IsOpenApi2xReferenced(VogenKnownSymbols vogenKnownSymbols) => vogenKnownSymbols.JsonSchemaType is not null;

    public static OpenApiVersionBeingUsed DetermineOpenApiVersionBeingUsed(VogenKnownSymbols knownSymbols)
    {
        if (IsOpenApi2xReferenced(knownSymbols))
        {
            return OpenApiVersionBeingUsed.TwoPlus;
        }

        if (IsOpenApi1xReferenced(knownSymbols))
        {
            return OpenApiVersionBeingUsed.One;
        }

        return OpenApiVersionBeingUsed.None;
    }
}