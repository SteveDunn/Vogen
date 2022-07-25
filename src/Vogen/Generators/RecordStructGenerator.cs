using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators;

public class RecordStructGenerator : IGenerateSourceCode
{
    public string BuildClass(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var structName = tds.Identifier;
        var itemUnderlyingType = item.UnderlyingType?.ToString() ?? "global::System.Int32";

        return $@"
using Vogen;

{Util.WriteStartNamespace(item.FullNamespace)}
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{Util.GenerateYourAssemblyName()}"", ""{Util.GenerateYourAssemblyVersion()}"")]
    {Util.GenerateAnyConversionAttributes(tds, item)}
    [global::System.Diagnostics.DebuggerTypeProxyAttribute(typeof({structName}DebugView))]
    [global::System.Diagnostics.DebuggerDisplayAttribute(""Underlying type: {itemUnderlyingType}, Value = {{ _value }}"")]
    { Util.GenerateModifiersFor(tds)} record struct {structName}
    {{
#if DEBUG    
        private readonly global::System.Diagnostics.StackTrace _stackTrace = null;
#endif

        private readonly global::System.Boolean _isInitialized;
        
        private readonly {itemUnderlyingType} _value;

        public readonly {itemUnderlyingType} Value
        {{
            [global::System.Diagnostics.DebuggerStepThroughAttribute]
            get
            {{
                EnsureInitialized();
                return _value;
            }}
            init
            {{
                {GenerateNullCheckIfNeeded(item)}

                {Util.GenerateNormalizeInputMethodIfNeeded(item)}

                {Util.GenerateValidation(item)}

                _value = value;
            }}
        }}

        [global::System.Diagnostics.DebuggerStepThroughAttribute]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        public {structName}()
        {{
#if DEBUG
            _stackTrace = new global::System.Diagnostics.StackTrace();
#endif

            _isInitialized = false;
            _value = default;
        }}

        [global::System.Diagnostics.DebuggerStepThroughAttribute]
        private {structName}({itemUnderlyingType} value) 
        {{
            _value = value;
            _isInitialized = true;
        }}

        /// <summary>
        /// Builds an instance from the provided underlying type.
        /// </summary>
        /// <param name=""value"">The underlying type.</param>
        /// <returns>An instance of this type.</returns>
        public static {structName} From({itemUnderlyingType} value)
        {{
            {Util.GenerateNormalizeInputMethodIfNeeded(item)}

            {structName} instance = new {structName}(value);

            {Util.GenerateValidation(item)}

            return instance;
        }}

        // only called internally when something has been deserialized into
        // its primitive type.
        private static {structName} Deserialize({itemUnderlyingType} value)
        {{
            {Util.GenerateNormalizeInputMethodIfNeeded(item)}

            {structName} instance = new {structName}(value);

            {Util.GenerateCallToValidateForDeserializing(item)}
            return instance;
        }}

        private readonly void EnsureInitialized()
        {{
            if (!_isInitialized)
            {{
#if DEBUG
                global::System.String message = ""Use of uninitialized Value Object at: "" + _stackTrace ?? """";
#else
                global::System.String message = ""Use of uninitialized Value Object."";
#endif

                throw new {item.ValidationExceptionFullName}(message);
            }}
        }}

        // record enumerates fields - we just want our Value and to throw if it's not initialized.
        {Util.GenerateToString(item)}

        { Util.GenerateAnyInstances(tds, item)}
 
        {Util.GenerateAnyConversionBodies(tds, item)}

        {Util.GenerateDebuggerProxyForStructs(tds, item)}

}}
{Util.WriteCloseNamespace(item.FullNamespace)}";
    }

    private string GenerateNullCheckIfNeeded(VoWorkItem voWorkItem) =>
        voWorkItem.IsValueType ? string.Empty
            : $@"            if (value is null)
            {{
                throw new {voWorkItem.ValidationExceptionFullName}(""Cannot create a value object with null."");
            }}
";
}