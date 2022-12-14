using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Vogen;

internal sealed class VogenConfigurationBuildResult
{
    public VogenConfiguration? ResultingConfiguration { get; set; }

    public List<Diagnostic> Diagnostics { get; set; } = new();

    public static VogenConfigurationBuildResult Null => new();

    public void AddDiagnostic(Diagnostic diagnostic) => Diagnostics.Add(diagnostic);
}