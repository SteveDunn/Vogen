using System.Diagnostics;

namespace Vogen;

/// <summary>
/// Controls schema filter generation for Swashbuckle.
/// If Swashbuckle.AspNetCore.SwaggerGen is not referenced, nothing will be generated.
/// </summary>
public enum SwashbuckleSchemaFilterGeneration
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Unspecified = -1,
    /// <summary>
    /// Do not generate the schema filter.
    /// </summary>
    Omit = 0,
    
    /// <summary>
    /// Generate the schema filter if Swashbuckle.AspNetCore.SwaggerGen is referenced.
    /// </summary>
    Generate = 1
}