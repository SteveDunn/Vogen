using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen;

internal class WriteStaticAbstracts
{
    public static void WriteIfNeeded(VogenConfiguration? globalConfig,
        SourceProductionContext context,
        Compilation compilation)
    {
        if (compilation is CSharpCompilation { LanguageVersion: < LanguageVersion.CSharp11 })
        {
            return;
        }

        StaticAbstractsGeneration generation = globalConfig?.StaticAbstractsGeneration ??
                                               VogenConfiguration.DefaultInstance.StaticAbstractsGeneration;

        if (generation == StaticAbstractsGeneration.Omit)
        {
            return;
        }

        string source =
            $"""
             {GeneratedCodeSegments.Preamble}

             {GenerateSource()}
             """;
        
        context.AddSource("VogenInterfaces_g.cs", source);

        string GenerateSource()
        {
            return $$"""
                     public interface IVogen<TSelf, TPrimitive>
                         where TSelf : IVogen<TSelf, TPrimitive>
                         where TPrimitive : notnull
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

                return """
                       static abstract TSelf From(TPrimitive value);
                       static abstract bool TryFrom(TPrimitive value, out TSelf vo);
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

            return """
                   static abstract bool operator ==(TSelf left, TSelf right);
                   static abstract bool operator !=(TSelf left, TSelf right);

                   static abstract bool operator ==(TSelf left, TPrimitive right);
                   static abstract bool operator !=(TSelf left, TPrimitive right);

                   static abstract bool operator ==(TPrimitive left, TSelf right);
                   static abstract bool operator !=(TPrimitive left, TSelf right);

                   """;
        }
    }

    public static string WriteHeaderIfNeeded(string precedingText, VoWorkItem item, TypeDeclarationSyntax tds)
    {
        if (item.StaticAbstractsGeneration == StaticAbstractsGeneration.Omit)
        {
            return string.Empty;
        }

        if (item.LanguageVersion <= LanguageVersion.CSharp11)
        {
            return string.Empty;
        }

        return precedingText + $" IVogen<{tds.Identifier}, {item.UnderlyingTypeFullName}>";
    }
}
