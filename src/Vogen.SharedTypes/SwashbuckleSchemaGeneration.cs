using System.Diagnostics;

namespace Vogen;

/// <summary>
/// Controls schema generation for Swashbuckle.
/// If Swashbuckle.AspNetCore.SwaggerGen is not referenced, nothing will be generated.
/// </summary>
public enum SwashbuckleSchemaGeneration
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Unspecified = -1,
    /// <summary>
    /// Do not generate anything related to OpenAPI schemas.
    /// </summary>
    Omit = 0,
    
    /// <summary>
    /// Generate a schema filter.
    /// </summary>
    GenerateSchemaFilter = 1,

    /// <summary>
    /// Generate extension method to map types with SwaggerGenOptions.
    /// </summary>
    GenerateExtensionMethodToMapTypesOnSwaggerGenOptions = 2
}