using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Vogen.Types;

namespace Vogen;

internal class GenerateCodeForBsonSerializers
{
    private const string _nameSuffix = "BsonSerializer";
    
    /// <summary>
    /// For each value object that has a BSON conversion attribute, write a separate file with a serializer,
    /// with a class name of '[WrapperName]BsonSerializer', and a filename of '[NameSpace].[WrapperName]_bson.g.cs' 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="compilation"></param>
    /// <param name="workItems"></param>
    public static void GenerateForApplicableValueObjects(SourceProductionContext context, Compilation compilation, List<VoWorkItem> workItems)
    {
        if (!compilation.IsAtLeastCSharp12())
        {
            return;
        }

        var applicableWrappers = workItems.Where(i => i.HasConversion(Conversions.Bson)).ToList();
        
        foreach (var eachWrapper in applicableWrappers)
        {
            string eachGenerated = GenerateSourceWithPreambleAndNamespace(eachWrapper);

            var filename = new Filename(eachWrapper.WrapperType.ToDisplayString() + "_bson.g.cs");

            Util.TryWriteUsingUniqueFilename(filename, context, Util.FormatSource(eachGenerated));
        }
        
        WriteRegistration(applicableWrappers, compilation, context);
    }

    /// <summary>
    /// For each marker class, write a separate file, with a type matching the FQN of the marker class appended with
    /// 'Bson', and for each attribute, generate the serializer as a nested classed
    /// </summary>
    /// <param name="context"></param>
    /// <param name="markerClass"></param>
    public static void GenerateForMarkerClasses(SourceProductionContext context, ImmutableArray<MarkerClassDefinition> markerClass)
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

        string pns = markerClassSymbol.FullUnalisaedNamespace();

        string ns = pns.Length == 0 ? "" : $"namespace {pns};";

        var isPublic = markerClassSymbol.DeclaredAccessibility.HasFlag(Accessibility.Public);
        var accessor = isPublic ? "public" : "internal";

        var sourceCode = $$"""
                  {{GeneratedCodeSegments.Preamble}}

                  {{ns}}

                  {{accessor}} partial class {{markerClassSymbol.Name}}
                  {
                     {{GenerateManifest()}}
                     {{GenerateSerializers()}}
                  }

                  """;

        string filename = Util.GetLegalFilenameForMarkerClass(markerClass.MarkerClassSymbol, ConversionMarkerKind.Bson);

        Util.TryWriteUsingUniqueFilename(filename, context, Util.FormatSource(sourceCode));

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
                        
                        return $"new {wrapperNameShort}{_nameSuffix}()";
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
                // ReSharper disable NullableWarningSuppressionIsUsed
                string generatedSource = GenerateSource(eachAttr.Marker!.VoSymbol, eachAttr.Marker!.UnderlyingTypeSymbol.FullAliasedNamespace(), "public");
                // ReSharper restore NullableWarningSuppressionIsUsed
                
                sb.AppendLine(
                    $$"""
                          {{generatedSource}}
                      """);
            }

            return sb.ToString();
        }
    }

    private static void WriteRegistration(List<VoWorkItem> items, Compilation compilation, SourceProductionContext context)
    {
        if (items.Count == 0)
        {
            return;
        }

        var classNameForRegistering = ClassNameForRegistering();

        string source =
            $$"""
              {{GeneratedCodeSegments.Preamble}}

              public static class {{classNameForRegistering}}
              {
                    static {{classNameForRegistering}}()
                    {
                        {{TextForEachRegisterCall(items)}}
                    }
                    
                    public static void TryRegister() { }
              }
              """;

        Util.TryWriteUsingUniqueFilename(classNameForRegistering, context, Util.FormatSource(source));
        return;

        string ClassNameForRegistering()
        {
            string projectName = ProjectName.FromAssemblyName(compilation.AssemblyName ?? "").Value;

            string s = "BsonSerializationRegister";
            if(projectName.Length > 0)
            {
                s = $"{s}For{projectName}";
            }

            return s;
        }
    }

    private static string TextForEachRegisterCall(List<VoWorkItem> wrappers)
    {
        StringBuilder sb = new();
        foreach (var eachWrapper in wrappers)
        {
            EscapedSymbolFullName n = new EscapedSymbolFullName(eachWrapper.WrapperType);

            string className = $"{n}{_nameSuffix}";

            sb.AppendLine(
                $"global::MongoDB.Bson.Serialization.BsonSerializer.TryRegisterSerializer(new {className}());");
        }

        return sb.ToString();
    }

    private static string GenerateSourceWithPreambleAndNamespace(VoWorkItem wrapper)
    {
        var isPublic = wrapper.WrapperType.DeclaredAccessibility.HasFlag(Accessibility.Public);
        var accessor = isPublic ? "public" : "internal";

        string fullNamespace = wrapper.FullUnaliasedNamespace;
        
        var ns = string.IsNullOrEmpty(fullNamespace) ? string.Empty : $"namespace {fullNamespace};";

        var generatedSource = GenerateSource(wrapper.WrapperType, wrapper.UnderlyingTypeFullNameWithGlobalAlias, accessor);

        return $"""
                {GeneratedCodeSegments.Preamble}

                {ns}

                {generatedSource}
                """;

    }

    private static string GenerateSource(INamedTypeSymbol wrapperSymbol, string underlyingFullName, string accessor)
    {
        var wrapperNames = new EscapedSymbolNames(wrapperSymbol);

        EscapedSymbolFullName wrapperFullName = wrapperNames.FullName;

        var className = $"{wrapperNames.ShortName}{_nameSuffix}";
        
        return
            $$"""
              {{accessor}} partial class {{className}} : global::MongoDB.Bson.Serialization.Serializers.SerializerBase<{{wrapperFullName}}>
              {
                  private readonly global::MongoDB.Bson.Serialization.IBsonSerializer<{{underlyingFullName}}> _serializer = global::MongoDB.Bson.Serialization.BsonSerializer.LookupSerializer<{{underlyingFullName}}>();
              
                  public override {{wrapperFullName}} Deserialize(global::MongoDB.Bson.Serialization.BsonDeserializationContext context, global::MongoDB.Bson.Serialization.BsonDeserializationArgs args)
                  { 
                    var newArgs = new global::MongoDB.Bson.Serialization.BsonDeserializationArgs { NominalType = typeof({{underlyingFullName}}) };
              
                    return Deserialize(_serializer.Deserialize(context, newArgs));
                  }
              
                  public override void Serialize(global::MongoDB.Bson.Serialization.BsonSerializationContext context, global::MongoDB.Bson.Serialization.BsonSerializationArgs args, {{wrapperFullName}} value) => 
                    _serializer.Serialize(context, args, value.Value);
                    
                static {{wrapperFullName}} Deserialize({{underlyingFullName}} value) => UnsafeDeserialize(default, value);
                
                [global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.StaticMethod, Name = "__Deserialize")]
                static extern {{wrapperFullName}} UnsafeDeserialize({{wrapperFullName}} @this, {{underlyingFullName}} value);      
                    
              }
              """;
    }
}