using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Vogen.Generator.Diagnostics;

namespace Vogen.Generator.generators
{
    [Generator]
    public class ValueObjectGenerator : ISourceGenerator
    {
        private readonly List<string> _log = new();

        private readonly ClassGeneratorForReferenceType _classGeneratorForReferenceType;
        private readonly ClassGeneratorForValueType _classGeneratorForValueType;
        private readonly StructGeneratorForValueAndReferenceTypes _structGeneratorForValueAndReferenceTypes;

        public ValueObjectGenerator()
        {
            _classGeneratorForReferenceType = new ClassGeneratorForReferenceType();
            _classGeneratorForValueType = new ClassGeneratorForValueType();
            _structGeneratorForValueAndReferenceTypes = new StructGeneratorForValueAndReferenceTypes();
        }

        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                // Debugger.Launch();
            }
#endif             
            // Register a factory that can create our custom syntax receiver
            context.RegisterForSyntaxNotifications(() => new ValueObjectReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver == null)
            {
                _log.Add("context.SyntaxContextReceiver was null.");

                return;
            }

            try
            {
                // the generator infrastructure will create a receiver and populate it
                // we can retrieve the populated instance via the context
                ValueObjectReceiver syntaxReceiver = (ValueObjectReceiver)context.SyntaxContextReceiver;

                var syntaxReceiverDiagnosticMessages = syntaxReceiver.DiagnosticMessages;
                if (syntaxReceiverDiagnosticMessages.HasErrors)
                {
                    ReportErrors(context, syntaxReceiverDiagnosticMessages);

                    return;
                }
                
                _log.Add($"Got {syntaxReceiver.WorkItems.Count} work item(s)");

                context.AddSource("ValueObjectReceiverLogs",
                    SourceText.From(
                        $@"/*{Environment.NewLine + string.Join(Environment.NewLine, syntaxReceiver.Log) + Environment.NewLine}*/",
                        Encoding.UTF8));

                foreach (var syntaxReceiverWorkItem in syntaxReceiver.WorkItems)
                {
                    HandleWorkItem(context, syntaxReceiverWorkItem);
                }
            }
            catch (Exception ex)
            {
                _log.Add(ex.ToString());
            }
            finally
            {
                context.AddSource("ValueObjectGeneratorLogs", SourceText.From(
                    $@"/*{Environment.NewLine + string.Join(Environment.NewLine, _log) + Environment.NewLine}*/",
                    Encoding.UTF8));
            }
        }

        private static void ReportErrors(GeneratorExecutionContext context,
            DiagnosticCollection syntaxReceiverDiagnosticMessages)
        {
            foreach (var eachDiag in syntaxReceiverDiagnosticMessages)
            {
                context.ReportDiagnostic(eachDiag);
            }
        }

        private void HandleWorkItem(GeneratorExecutionContext context, ValueObjectWorkItem item)
        {
            try
            {
                _log.Add($"generating for {item.TypeToAugment.Identifier}");

                // get the recorded user class
                TypeDeclarationSyntax voClass = item.TypeToAugment;

                _log.Add($"voClass.Identifier.ToFullString() is '{voClass.Identifier}'");

                IGenerateSourceCode generator = GetGenerator(item);

                string? classAsText = generator.BuildClass(item, voClass, _log);

                SourceText sourceText = SourceText.From(classAsText, Encoding.UTF8);
                

                string filename = $"{item.FullNamespace}_{voClass.Identifier}.Generated.cs";

                _log.Add($"-=-= writing source to {filename}");
                context.AddSource(filename, sourceText);

                _log.Add("finished that one...");
            }
            catch (Exception ex)
            {
                DiagnosticCollection c = new DiagnosticCollection();
                ReportErrors(context, c);
                _log.Add(ex.ToString());
            }
        }

        private IGenerateSourceCode GetGenerator(ValueObjectWorkItem item)
        {
            return item.TypeToAugment switch
            {
                ClassDeclarationSyntax when item.IsValueType => _classGeneratorForValueType,
                ClassDeclarationSyntax => _classGeneratorForReferenceType,
                StructDeclarationSyntax => _structGeneratorForValueAndReferenceTypes,
                _ => throw new InvalidOperationException("Don't know how to get the generator!")
            };
        }
    }
}