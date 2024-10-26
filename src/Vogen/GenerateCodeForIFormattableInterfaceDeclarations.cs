using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

public static class GenerateCodeForIFormattableInterfaceDeclarations
{
    public static string GenerateIfNeeded(string precedingText, VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var info = item.FormattableInformation;

        StringBuilder sb = new StringBuilder();

        if (item.FormattableInformation.UnderlyingDerivesFromIFormattable)
        {
            sb.Append($"{precedingText} global::System.IFormattable");
        }
        
        if (info.UnderlyingDerivesFromISpanFormattable)
        {
            sb.Append($"{precedingText} global::System.ISpanFormattable");
        }

        if (info.UnderlyingDerivesFromIUtf8SpanFormattable)
        {
            sb.Append($"{precedingText} global::System.IUtf8SpanFormattable");
        }

        return sb.ToString() ;
    }
}