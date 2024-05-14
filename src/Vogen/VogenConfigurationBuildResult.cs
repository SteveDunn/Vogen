using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Vogen;

internal sealed class VogenConfigurationBuildResult
{
    public VogenConfigurationBuildResult(VogenConfiguration? resultingConfiguration, IEnumerable<Diagnostic> diagnostics)
    {
        ResultingConfiguration = resultingConfiguration;
        Diagnostics = diagnostics;
    }
    
    public VogenConfiguration? ResultingConfiguration { get;  }

    public IEnumerable<Diagnostic> Diagnostics { get; }

    public static VogenConfigurationBuildResult Null => new(null, []);
    
    public bool HasDiagnostics => Diagnostics.Any();
}