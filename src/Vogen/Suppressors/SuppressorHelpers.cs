using System.Threading;
using Microsoft.CodeAnalysis;

namespace Vogen.Suppressors;

internal static class SuppressorHelpers
{
    public static SyntaxNode? TryFindNode(this Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        return TryFindNode(diagnostic.Location, cancellationToken);
    }

    private static SyntaxNode? TryFindNode(Location? location, CancellationToken cancellationToken)
    {
        if (location == null)
            return null;

        var syntaxTree = location.SourceTree;
        if (syntaxTree is null)
            return null;

        var root = syntaxTree.GetRoot(cancellationToken);
        return root.FindNode(location.SourceSpan);
    }
}