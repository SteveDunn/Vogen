using Vogen.Generators;

namespace Vogen;

public class GenerateCodeForXmlSerializable
{
    public static string GenerateInterfaceDefinitionIfNeeded(string precedingText, VoWorkItem item)
    {
        if (!item.Config.Conversions.HasFlag(Conversions.XmlSerializable))
        {
            return string.Empty;
        }

        return precedingText + $" global::System.Xml.Serialization.IXmlSerializable";
    }

    public static string GenerateBodyIfNeeded(GenerationParameters parameters)
    {
        if (!parameters.WorkItem.Config.Conversions.HasFlag(Conversions.XmlSerializable))
        {
            return string.Empty;
        }
        
        return $$"""
            public global::System.Xml.Schema.XmlSchema? GetSchema() => null!;
            
            public void ReadXml(global::System.Xml.XmlReader reader) 
            {
                _value = ({{parameters.WorkItem.UnderlyingTypeFullNameWithGlobalAlias}})reader.ReadElementContentAs(typeof({{parameters.WorkItem.UnderlyingTypeFullNameWithGlobalAlias}}), null!);
                #if !VOGEN_NO_VALIDATION
                    _isInitialized = true;
                #endif
            }
            
            public void WriteXml(global::System.Xml.XmlWriter writer) => writer.WriteValue(_value);
            """;
    }
}