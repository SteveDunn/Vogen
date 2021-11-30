using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Vogen.Generator.Diagnostics;
using Vogen.Generator.Generators;

namespace Vogen.Generator
{
    [Generator]
    public class ValueObjectGenerator : ISourceGenerator
    {
        private readonly List<string> _log = new();

        private readonly ClassGeneratorForReferenceType _classGeneratorForReferenceType;
        private readonly ClassGeneratorForValueType _classGeneratorForValueType;
        private readonly StructGeneratorForValueAndReferenceTypes _structGeneratorForValueAndReferenceTypes;
        
        private const string _vogenSharedTypes = @"
using System;
using System.Runtime.Serialization;

namespace Vogen
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class ValueObjectAttribute : Attribute
    {
        public Type UnderlyingType { get; }

        public ValueObjectAttribute(Type underlyingType)
        {
            UnderlyingType = underlyingType;
        }
    }

    public class Validation
    {
        public string ErrorMessage { get; }

        public static readonly Validation Ok = new Validation(string.Empty);

        private Validation(string reason) => ErrorMessage = reason;

        public static Validation Invalid(string reason = """")
        {
            if (string.IsNullOrEmpty(reason))
            {
                return new Validation(""[none provided]"");
            }

            return new Validation(reason);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class InstanceAttribute : Attribute
    {
        public object Value { get; }

        public string Name { get; }

        public InstanceAttribute(string name, object value) => (Name, Value) = (name, value);
    }

    [Serializable]
    public class ValueObjectValidationException : Exception
    {
        public ValueObjectValidationException()
        {
        }

        public ValueObjectValidationException(string message) : base(message)
        {
        }

        public ValueObjectValidationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ValueObjectValidationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}";

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
            // Register the attribute source
            context.RegisterForPostInitialization((i) => i.AddSource("VogenSharedTypes", _vogenSharedTypes));

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
                INamedTypeSymbol voAttributeSymbol = context.Compilation.GetTypeByMetadataName("Vogen.ValueObjectAttribute")!;
                INamedTypeSymbol voInstanceSymbol = context.Compilation.GetTypeByMetadataName("Vogen.InstanceAttribute")!;

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

                foreach (var eachWorkItem in syntaxReceiver.WorkItems)
                {
                    HandleWorkItem(context, eachWorkItem);
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

                string classAsText = generator.BuildClass(item, voClass, _log);

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