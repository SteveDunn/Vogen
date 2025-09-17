using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Vogen.Diagnostics;
// ReSharper disable NullableWarningSuppressionIsUsed

namespace Vogen
{
    internal static class BuildInstanceProperties
    {
        public static IEnumerable<InstanceProperties?> Build(
            IEnumerable<AttributeData> allAttributes,
            SourceProductionContext context,
            INamedTypeSymbol voClass,
            INamedTypeSymbol underlyingType,
            VogenKnownSymbols knownSymbols)
        {
            return allAttributes.Select(a => Build(a, context, voClass, underlyingType, knownSymbols));
        }

        private static InstanceProperties? Build(
            AttributeData matchingAttribute,
            SourceProductionContext context,
            INamedTypeSymbol voClass,
            INamedTypeSymbol underlyingType,
            VogenKnownSymbols knownSymbols)
        {
            // try build it from non-named arguments

            if (!matchingAttribute.ConstructorArguments.IsEmpty)
            {
                // make sure we don't have any errors
                ImmutableArray<TypedConstant> args = matchingAttribute.ConstructorArguments;

                foreach (TypedConstant arg in args)
                {
                    if (arg.Kind == TypedConstantKind.Error)
                    {
                        return null;
                    }
                }

                return TryBuild(args[0], args[1], args[2], voClass, context, underlyingType, knownSymbols);
            }

            // try build it from named arguments
            if (!matchingAttribute.NamedArguments.IsEmpty)
            {
                TypedConstant nameConstant = default;
                TypedConstant valueConstant = default;
                TypedConstant commentConstant = default;

                foreach (KeyValuePair<string, TypedConstant> arg in matchingAttribute.NamedArguments)
                {
                    TypedConstant typedConstant = arg.Value;
                    if (typedConstant.Kind == TypedConstantKind.Error)
                    {
                        return null;
                    }

                    switch (arg.Key)
                    {
                        case "name":
                            nameConstant = typedConstant;
                            break;
                        case "value":
                            valueConstant = typedConstant;
                            break;
                        case "tripleSlashComments":
                            commentConstant = typedConstant;
                            break;
                    }
                }

                return TryBuild(nameConstant, valueConstant, commentConstant, voClass, context, underlyingType, knownSymbols);
            }

            return null;
        }

        private static InstanceProperties? TryBuild(TypedConstant nameConstant,
            TypedConstant valueConstant,
            TypedConstant commentConstant,
            INamedTypeSymbol voClass,
            SourceProductionContext context,
            INamedTypeSymbol underlyingType,
            VogenKnownSymbols knownSymbols)
        {
            bool hasErrors = false;
            if (nameConstant.Value is null)
            {
                context.ReportDiagnostic(DiagnosticsCatalogue.InstanceMethodCannotHaveNullArgumentName(voClass));
                hasErrors = true;
            }

            if (valueConstant.Value is null)
            {
                context.ReportDiagnostic(DiagnosticsCatalogue.InstanceMethodCannotHaveNullArgumentValue(voClass));
                hasErrors = true;
            }

            if (hasErrors)
            {
                return null;
            }

            var r = InstanceGeneration.TryBuildInstanceValueAsText(
                (string)nameConstant.Value!, 
                valueConstant.Value!, underlyingType.EscapedFullName(),
                knownSymbols);

            if (!r.Success)
            {
                context.ReportDiagnostic(DiagnosticsCatalogue.InstanceValueCannotBeConverted(voClass, r.ErrorMessage));
                return null;
            }

            return new InstanceProperties(
                (string)nameConstant.Value!, 
                r.Value,
                valueConstant.Value!, 
                (string) (commentConstant.Value ?? string.Empty));
        }
    }
}