using System;
using Vogen;

[assembly: VogenDefaults(openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateSwashbuckleMappingExtensionMethod | OpenApiSchemaCustomizations.GenerateOpenApiMappingExtensionMethod)]

// simple program
Console.WriteLine("Hello, World!");

[ValueObject<string>]
public partial class StringType;
