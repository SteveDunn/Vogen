using Vogen;

[assembly: VogenDefaults(
    openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateSwashbuckleMappingExtensionMethod)]

namespace WebApplication.Shared;


[ValueObject]
public partial struct SharedStruct;