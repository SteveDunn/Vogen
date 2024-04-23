using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Generators.Conversions;

namespace Vogen.Generators;

public class RecordClassGenerator : IGenerateSourceCode
{
    public string BuildClass(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var className = tds.Identifier;

        var itemUnderlyingType = item.UnderlyingTypeFullName;

        return $@"
using Vogen;

{Util.WriteStartNamespace(item.FullNamespace)}
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] 
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{Util.GenerateYourAssemblyName()}"", ""{Util.GenerateYourAssemblyVersion()}"")]
    {Util.GenerateAnyConversionAttributes(tds, item)}
    {DebugGeneration.GenerateDebugAttributes(item, className, itemUnderlyingType)}
    {Util.GenerateModifiersFor(tds)} record class {className} : global::System.IEquatable<{className}>, global::System.IEquatable<{itemUnderlyingType}>{GenerateComparableCode.GenerateIComparableHeaderIfNeeded(", ", item, tds)}{GenerateCodeForIParsableInterfaceDeclarations.GenerateIfNeeded(", ", item, tds)}
    {{
{DebugGeneration.GenerateStackTraceFieldIfNeeded(item)}
        private readonly global::System.Boolean _isInitialized;
        private readonly {itemUnderlyingType} _value;
        
        /// <summary>
        /// Gets the underlying <see cref=""{itemUnderlyingType}"" /> value if set, otherwise a <see cref=""{nameof(ValueObjectValidationException)}"" /> is thrown.
        /// </summary>
        public {itemUnderlyingType} Value
        {{
            [global::System.Diagnostics.DebuggerStepThroughAttribute]
            get
            {{
                EnsureInitialized();
                return _value;
            }}
            [global::System.Diagnostics.DebuggerStepThroughAttribute]
            init
            {{
                {GenerateNullCheckIfNeeded(item)}

                {Util.GenerateCallToNormalizeMethodIfNeeded(item)}

                {Util.GenerateCallToValidation(item)}

                _value = value;
            }}
        }}

{GenerateStaticConstructor.GenerateIfNeeded(item)}
        [global::System.Diagnostics.DebuggerStepThroughAttribute]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        public {className}()
        {{
#if DEBUG
            {DebugGeneration.SetStackTraceIfNeeded(item)}
#endif
            _isInitialized = false;
            _value = default;
        }}

        [global::System.Diagnostics.DebuggerStepThroughAttribute]
        private {className}({itemUnderlyingType} value)
        {{
            _value = value;
            _isInitialized = true;
        }}

        /// <summary>
        /// Builds an instance from the provided underlying type.
        /// </summary>
        /// <param name=""value"">The underlying type.</param>
        /// <returns>An instance of this type.</returns>
        public static {className} From({itemUnderlyingType} value)
        {{
            {GenerateNullCheckIfNeeded(item)}

            {Util.GenerateCallToNormalizeMethodIfNeeded(item)}

            {Util.GenerateCallToValidation(item)}

            {className} instance = new {className}(value);

            return instance;
        }}
{GenerateEqualsAndHashCodes.GenerateStringComparersIfNeeded(item, tds)}  
        // only called internally when something has been deserialized into
        // its primitive type.
        private static {className} Deserialize({itemUnderlyingType} value)
        {{
            {GenerateNullCheckIfNeeded(item)}

            {Util.GenerateCallToNormalizeMethodIfNeeded(item)}

            {Util.GenerateCallToValidateForDeserializing(item)}

            return new {className}(value);
        }}
        {GenerateEqualsAndHashCodes.GenerateEqualsForAClass(item, tds)}

        public static global::System.Boolean operator ==({className} left, {itemUnderlyingType} right) => Equals(left.Value, right);
        public static global::System.Boolean operator !=({className} left, {itemUnderlyingType} right) => !Equals(left.Value, right);

        public static global::System.Boolean operator ==({itemUnderlyingType} left, {className} right) => Equals(left, right.Value);
        public static global::System.Boolean operator !=({itemUnderlyingType} left, {className} right) => !Equals(left, right.Value);

{GenerateCastingOperators.Generate(item,tds)}
        {GenerateComparableCode.GenerateIComparableImplementationIfNeeded(item, tds)}

        {GenerateCodeForTryParse.GenerateAnyHoistedTryParseMethods(item)}{GenerateCodeForParse.GenerateAnyHoistedParseMethods(item)}

{GenerateEqualsAndHashCodes.GenerateGetHashCodeForAClass(item)}

        private void EnsureInitialized()
        {{
            if (!_isInitialized)
            {{
#if DEBUG
                {DebugGeneration.GenerateMessageForUninitializedValueObject(item)}
#else
                global::System.String message = ""Use of uninitialized Value Object."";
#endif

                throw new {item.ValidationExceptionFullName}(message);
            }}
        }}


        {InstanceGeneration.GenerateAnyInstances(tds, item)}

        // record enumerates fields - we just want our Value and to throw if it's not initialized.
        {Util.GenerateToString(item)}

        {Util.GenerateAnyConversionBodies(tds, item)}

        {Util.GenerateDebuggerProxyForClasses(tds, item)}
    }}
{GenerateEfCoreExtensions.GenerateIfNeeded(item)}
{Util.WriteCloseNamespace(item.FullNamespace)}";
    }

    private static string GenerateNullCheckIfNeeded(VoWorkItem voWorkItem) =>
        voWorkItem.IsTheUnderlyingAValueType ? string.Empty
            : $@"            if (value is null)
            {{
                throw new {voWorkItem.ValidationExceptionFullName}(""Cannot create a value object with null."");
            }}
";
}