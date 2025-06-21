using System;
using System.Diagnostics;

namespace Vogen;

/// <summary>
/// Controls schema customization.
/// </summary>
[Flags]
public enum OpenApiSchemaCustomizations
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Unspecified = -1,

    /// <summary>
    /// No customization.
    /// </summary>
    Omit = 0,

    /// <summary>
    /// Generate a schema filter.
    /// If Swashbuckle.AspNetCore.SwaggerGen is not referenced, nothing will be generated.
    /// </summary>
    GenerateSwashbuckleSchemaFilter = 1 << 0,

    /// <summary>
    /// Generate extension method to map types with SwaggerGenOptions.
    /// If Swashbuckle.AspNetCore.SwaggerGen is not referenced, nothing will be generated.
    /// </summary>
    GenerateSwashbuckleMappingExtensionMethod = 1 << 1,

    /// <summary>
    /// Generate extension method to map types with Microsoft.AspNetCore.OpenApi.OpenApiOptions.
    /// If Microsoft.AspNetCore.OpenApi is not referenced, nothing will be generated.
    /// </summary>
    GenerateOpenApiMappingExtensionMethod = 1 << 2,
}