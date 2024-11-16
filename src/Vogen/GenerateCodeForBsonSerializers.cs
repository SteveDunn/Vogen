using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Vogen.Types;

namespace Vogen;

internal class GenerateCodeForBsonSerializers
{
    /// <summary>
    /// For each value object that has a BSON conversion attribute, write a separate file with a serializer,
    /// with a class name of '[WrapperName]BsonSerializer', and a filename of '[NameSpace].[WrapperName]_bson.g.cs' 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="compilation"></param>
    /// <param name="workItems"></param>
    public static void GenerateForApplicableValueObjects(SourceProductionContext context, Compilation compilation, List<VoWorkItem> workItems)
    {
        if (!compilation.IsAtLeastCSharpVersion(LanguageVersion.CSharp12))
        {
            return;
        }

        List<GeneratedSource> serializersToWrite = workItems.Where(i => i.HasConversion(Conversions.Bson)).Select(
            wo => GenerateSource(wo.FullNamespace, wo.WrapperType, wo.UnderlyingType)).ToList();
        
        foreach (var eachToWrite in serializersToWrite)
        {
            SourceText sourceText = SourceText.From(eachToWrite.SourceCode, Encoding.UTF8);

            Util.TryWriteUsingUniqueFilename(eachToWrite.FileName, context, sourceText);
        }
        
        WriteRegisterer(serializersToWrite, compilation, context);
    }

    /// <summary>
    /// For each marker class, write a separate file, with a type matching the FQN of the marker class appended with
    /// 'Bson', and for each attribute, generate the serializer as a nested classed
    /// </summary>
    /// <param name="context"></param>
    /// <param name="compilation"></param>
    /// <param name="markerClass"></param>
    public static void GenerateForMarkerClasses(SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<MarkerClassDefinition> markerClass)
    {
        foreach (var each in markerClass)
        {
            GenerateForMarkerClass(context, each);
        }
    }

    private static void GenerateForMarkerClass(SourceProductionContext context, MarkerClassDefinition markerClass)
    {
        var markerClassSymbol = markerClass.MarkerClassSymbol;

        if (!markerClass.AttributeDefinitions.Any(m => m.Marker?.Kind is ConversionMarkerKind.Bson))
        {
            return;
        }

        string pns = markerClassSymbol.FullNamespace();

        string ns = pns.Length == 0 ? "" : $"namespace {pns};";

        var isPublic = markerClassSymbol.DeclaredAccessibility.HasFlag(Accessibility.Public);
        var accessor = isPublic ? "public" : "internal";

        var s = $$"""
                  {{GeneratedCodeSegments.Preamble}}

                  {{ns}}

                  {{accessor}} partial class {{markerClassSymbol.Name}}
                  {
                     {{GenerateManifest()}}
                     {{GenerateSerializers()}}
                  }

                  """;

        SourceText sourceText = Util.FormatSource(s);

        string filename = Util.GetLegalFilenameForMarkerClass(markerClass.MarkerClassSymbol, ConversionMarkerKind.Bson);

        Util.TryWriteUsingUniqueFilename(filename, context, sourceText);

        return;

        string GenerateManifest()
        {
            return
                $$"""
                  {{accessor}} static global::MongoDB.Bson.Serialization.IBsonSerializer[] BsonSerializers => new global::MongoDB.Bson.Serialization.IBsonSerializer[]
                  {
                      {{GenerateEach()}}
                  };
                  """;

            string GenerateEach()
            {
                string?[] names = markerClass.AttributeDefinitions.Where(
                    m => m.Marker?.Kind is ConversionMarkerKind.Bson).Select(
                    x =>
                    {
                        if (x.Marker is null)
                        {
                            return null;
                        }

                        string wrapperNameShort = x.Marker.VoSymbol.Name;
                        
                        return $"new {wrapperNameShort}BsonSerializer()";
                    }).ToArray();

                return string.Join(", ", names);
            }
        }

        string GenerateSerializers()
        {
            StringBuilder sb = new();

            foreach (MarkerAttributeDefinition eachAttr in markerClass.AttributeDefinitions.Where(
                         m => m.Marker?.Kind is ConversionMarkerKind.Bson))
            {
                var generatedSource = GenerateSource(eachAttr.Marker!.VoSymbol, eachAttr.Marker!.UnderlyingTypeSymbol, "public");
                sb.AppendLine(
                    $$"""
                          {{generatedSource.SourceCode}}
                      """);
            }

            return sb.ToString();
        }
    }

    private static void WriteRegisterer(List<GeneratedSource> items, Compilation compilation, SourceProductionContext context)
    {
        if (items.Count == 0)
        {
            return;
        }


        var assemblyName = new SanitizedAssemblyName(compilation.AssemblyName);

        string className = $"BsonSerializationRegister";
        if(assemblyName.Value.Length > 0)
        {
            className = className + "For" + assemblyName;
        }

        string source =
            $$"""
              {{GeneratedCodeSegments.Preamble}}

              public static class {{className}}
              {
                    static {{className}}()
                    {
                        {{TextForEachRegisterCall(items)}}
                    }
                    
                    public static void TryRegister() { }
              }
              """;
        
        SourceText sourceText = SourceText.From(source, Encoding.UTF8);

        Util.TryWriteUsingUniqueFilename(className, context, sourceText);
    }

    private static string TextForEachRegisterCall(List<GeneratedSource> items)
    {
        StringBuilder sb = new();
        foreach (var eachEntry in items)
        {
            sb.AppendLine(
                $"global::MongoDB.Bson.Serialization.BsonSerializer.TryRegisterSerializer(new {eachEntry.FullyQualifiedClassName}());");
        }

        return sb.ToString();
    }

    private static GeneratedSource GenerateSource(
        string fullNamespace,
        INamedTypeSymbol wrapperSymbol,
        INamedTypeSymbol underlyingSymbol)
    {
        var isPublic = wrapperSymbol.DeclaredAccessibility.HasFlag(Accessibility.Public);
        var accessor = isPublic ? "public" : "internal";
        var ns = string.IsNullOrEmpty(fullNamespace) ? string.Empty : $"namespace {fullNamespace};";

        var generatedSource = GenerateSource(wrapperSymbol, underlyingSymbol, accessor);

        return new(
            generatedSource.ClassName,
            generatedSource.FullyQualifiedClassName,
            generatedSource.FileName,
            $"""
             {GeneratedCodeSegments.Preamble}

             {ns}

             {generatedSource.SourceCode}
             """);
    }

    private record GeneratedSource(string ClassName, string FullyQualifiedClassName, Filename FileName, string SourceCode);

    private static GeneratedSource GenerateSource(INamedTypeSymbol wrapperSymbol, INamedTypeSymbol underlyingSymbol, string accessor)
    {
        var wrapperName = new EscapedSymbolNames(wrapperSymbol);

        string underlyingTypeName = Util.EscapeKeywordsIfRequired(underlyingSymbol.FullName());

        string className = $"{Util.EscapeKeywordsIfRequired(wrapperSymbol.Name)}BsonSerializer";
        string fullyQualifiedClassName = $"{Util.EscapeKeywordsIfRequired(wrapperSymbol.FullName())}BsonSerializer";

        Filename filename = new Filename(wrapperSymbol.ToDisplayString() + "_bson.g.cs");

        return new(
            className,
            fullyQualifiedClassName,
            filename,
            $$"""
              {{accessor}} partial class {{className}} : global::MongoDB.Bson.Serialization.Serializers.SerializerBase<{{wrapperName.FullName}}>
              {
                  private readonly global::MongoDB.Bson.Serialization.IBsonSerializer<{{underlyingTypeName}}> _serializer = global::MongoDB.Bson.Serialization.BsonSerializer.LookupSerializer<{{underlyingTypeName}}>();
              
                  public override {{wrapperName.FullName}} Deserialize(global::MongoDB.Bson.Serialization.BsonDeserializationContext context, global::MongoDB.Bson.Serialization.BsonDeserializationArgs args)
                  { 
                    var newArgs = new global::MongoDB.Bson.Serialization.BsonDeserializationArgs { NominalType = typeof({{underlyingTypeName}}) };
              
                    return Deserialize(_serializer.Deserialize(context, newArgs));
                  }
              
                  public override void Serialize(global::MongoDB.Bson.Serialization.BsonSerializationContext context, global::MongoDB.Bson.Serialization.BsonSerializationArgs args, {{wrapperName.FullName}} value) => 
                    _serializer.Serialize(context, args, value.Value);
                    
                static {{wrapperName.FullName}} Deserialize({{underlyingTypeName}} value) => UnsafeDeserialize(default, value);
                
                [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticMethod, Name = "__Deserialize")]
                static extern {{wrapperName.FullName}} UnsafeDeserialize({{wrapperName.FullName}} @this, {{underlyingTypeName}} value);      
                    
              }
              """);
    }
}