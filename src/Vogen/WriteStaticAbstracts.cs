using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

internal class WriteStaticAbstracts
{
    public static void WriteInterfacesAndMethodsIfNeeded(VogenConfiguration? globalConfig,
        SourceProductionContext context,
        Compilation compilation)
    {
        if(!compilation.IsAtLeastCSharpVersion(LanguageVersion.CSharp11))
        {
            return;
        }

        StaticAbstractsGeneration generation = globalConfig?.StaticAbstractsGeneration ??
                                               VogenConfiguration.DefaultInstance.StaticAbstractsGeneration;

        // we don't want to generate if omitting or if we're only _using_ the interface (it could be declared somewhere else)
        if (generation is StaticAbstractsGeneration.Omit or StaticAbstractsGeneration.ValueObjectsDeriveFromTheInterface)
        {
            return;
        }

        bool nullableEnabled =
            (compilation.Options as CSharpCompilationOptions)!.NullableContextOptions.HasFlag(NullableContextOptions.Enable);

        string source = $"""
                         {GeneratedCodeSegments.Preamble}

                         {GenerateSource()}
                         """;

        if (nullableEnabled)
        {
            source = $"""
                      #nullable enable
                      {source}
                      #nullable restore
                      """;
        }
        
        context.AddSource("VogenInterfaces_g.cs", Util.FormatSource(source));

        string GenerateSource()
        {
            return $$"""
                     public interface IVogen<TSelf, TPrimitive>
                         where TSelf : IVogen<TSelf, TPrimitive>
                     {
                         {{GenerateCastingOperatorsIfNeeded()}}
                         {{GenerateEqualsOperatorsIfNeeded()}}
                     
                         {{GenerateFactoryMethodsIfNeeded()}}
                     
                         {{GenerateInstancePropertiesAndMethodsIfNeeded()}}
                     }
                     """;

            string GenerateInstancePropertiesAndMethodsIfNeeded()
            {
                if (!generation.HasFlag(StaticAbstractsGeneration.InstanceMethodsAndProperties))
                {
                    return string.Empty;
                }

                return """
                       TPrimitive Value { get; }
                       bool IsInitialized();
                       """;
            }

            string GenerateFactoryMethodsIfNeeded()
            {
                if (!generation.HasFlag(StaticAbstractsGeneration.FactoryMethods))
                {
                    return string.Empty;
                }

                string questionMarkForTSelf = nullableEnabled ? "?" : string.Empty;
                
                return $"""
                        static abstract TSelf From(TPrimitive value);
                        static abstract bool TryFrom(TPrimitive value, out TSelf{questionMarkForTSelf} vo);
                        """;
            }
        }


        string GenerateCastingOperatorsIfNeeded()
        {
            StringBuilder sb = new StringBuilder();

            if (generation.HasFlag(StaticAbstractsGeneration.ExplicitCastFromPrimitive))
            {
                sb.AppendLine("""static abstract explicit operator TSelf(TPrimitive value);""");
            }

            if (generation.HasFlag(StaticAbstractsGeneration.ExplicitCastToPrimitive))
            {
                sb.AppendLine("""static abstract explicit operator TPrimitive(TSelf value);""");
            }

            if (generation.HasFlag(StaticAbstractsGeneration.ImplicitCastFromPrimitive))
            {
                sb.AppendLine("""static abstract implicit operator TSelf(TPrimitive value);""");
            }

            if (generation.HasFlag(StaticAbstractsGeneration.ImplicitCastToPrimitive))
            {
                sb.AppendLine("""static abstract implicit operator TPrimitive(TSelf value);""");
            }

            return sb.ToString();
        }


        string GenerateEqualsOperatorsIfNeeded()
        {
            if (!generation.HasFlag(StaticAbstractsGeneration.EqualsOperators))
            {
                return string.Empty;
            }
            
            string s = """
                   static abstract bool operator ==(TSelf left, TSelf right);
                   static abstract bool operator !=(TSelf left, TSelf right);
                   """;

            var primitiveOperatorGeneration = globalConfig?.PrimitiveEqualityGeneration ??
                                              VogenConfiguration.DefaultInstance.PrimitiveEqualityGeneration;

            if (primitiveOperatorGeneration.HasFlag(PrimitiveEqualityGeneration.GenerateOperators))
            {
                s = s + """

                        static abstract bool operator ==(TSelf left, TPrimitive right);
                        static abstract bool operator !=(TSelf left, TPrimitive right);

                        static abstract bool operator ==(TPrimitive left, TSelf right);
                        static abstract bool operator !=(TPrimitive left, TSelf right);
                        
                        """;
            }

            return s;
        }
    }

    public static string WriteHeaderIfNeeded(string precedingText, VoWorkItem item, TypeDeclarationSyntax tds)
    {
        if (item.LanguageVersion < LanguageVersion.CSharp11)
        {
            return string.Empty;
        }

        if (!item.Config.StaticAbstractsGeneration.HasFlag(StaticAbstractsGeneration.ValueObjectsDeriveFromTheInterface))
        {
            return string.Empty;
        }

        return precedingText + $" IVogen<{tds.Identifier}, {item.UnderlyingTypeFullName}>";
    }
}
