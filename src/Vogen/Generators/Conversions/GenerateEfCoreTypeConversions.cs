using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators.Conversions;

internal class GenerateEfCoreTypeConversions : IGenerateConversion
{
    public string GenerateAnyAttributes(TypeDeclarationSyntax tds, VoWorkItem item, VogenKnownSymbols knownSymbols) => string.Empty;


    public string GenerateAnyBody(TypeDeclarationSyntax tds, VoWorkItem item, VogenKnownSymbols knownSymbols)
    {
        if (!item.Config.Conversions.HasFlag(Vogen.Conversions.EfCoreValueConverter))
        {
            return string.Empty;
        }

        var code = GenerateEfCoreTypes.GenerateInner(item.UnderlyingType, item.IsTheWrapperAValueType, item.WrapperType);
        
        return $"""
                #nullable disable

                {code}

                #nullable restore
                """;
    }
}