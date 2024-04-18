using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Generators.Conversions;

namespace Vogen.Generators;

public class StructGenerator : IGenerateSourceCode
{
    public string BuildClass(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var structName = tds.Identifier;

        var itemUnderlyingType = item.UnderlyingTypeFullName;

        return $@"
using Vogen;

{Util.WriteStartNamespace(item.FullNamespace)}
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] 
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{Util.GenerateYourAssemblyName()}"", ""{Util.GenerateYourAssemblyVersion()}"")]
    {Util.GenerateAnyConversionAttributes(tds, item)}
    {DebugGeneration.GenerateDebugAttributes(item, structName, itemUnderlyingType)}
    { Util.GenerateModifiersFor(tds)} struct {structName} : global::System.IEquatable<{structName}>, global::System.IEquatable<{itemUnderlyingType}>{GenerateComparableCode.GenerateIComparableHeaderIfNeeded(", ", item, tds)}{GenerateCodeForIParsableInterfaceDeclarations.GenerateIfNeeded(", ", item, tds)}
    {{
{DebugGeneration.GenerateStackTraceFieldIfNeeded(item)}

        private readonly global::System.Boolean _isInitialized;
        
        private readonly {itemUnderlyingType} _value;

        /// <summary>
        /// Gets the underlying <see cref=""{itemUnderlyingType}"" /> value if set, otherwise a <see cref=""{nameof(ValueObjectValidationException)}"" /> is thrown.
        /// </summary>
        public readonly {itemUnderlyingType} Value
        {{
            [global::System.Diagnostics.DebuggerStepThroughAttribute]
            get
            {{
                EnsureInitialized();
                return _value;
            }}
        }}

        [global::System.Diagnostics.DebuggerStepThroughAttribute]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        public {structName}()
        {{
#if DEBUG
            {DebugGeneration.SetStackTraceIfNeeded(item)}
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
            {Util.GenerateCallToNormalizeMethodIfNeeded(item)}

            {Util.GenerateCallToValidation(item)}

            {structName} instance = new {structName}(value);

            return instance;
        }}
{GenerateEqualsAndHashCodes.GenerateStringComparersIfNeeded(item, tds)}        

{GenerateCastingOperators.Generate(item,tds)}
        // only called internally when something has been deserialized into
        // its primitive type.
        private static {structName} Deserialize({itemUnderlyingType} value)
        {{
            {Util.GenerateCallToNormalizeMethodIfNeeded(item)}

            {Util.GenerateCallToValidateForDeserializing(item)}

            return new {structName}(value);
        }}
        {GenerateEqualsAndHashCodes.GenerateEqualsForAStruct(item, tds)}

        public static global::System.Boolean operator ==({structName} left, {structName} right) => Equals(left, right);
        public static global::System.Boolean operator !=({structName} left, {structName} right) => !(left == right);

        public static global::System.Boolean operator ==({structName} left, {itemUnderlyingType} right) => Equals(left.Value, right);
        public static global::System.Boolean operator !=({structName} left, {itemUnderlyingType} right) => !Equals(left.Value, right);

        public static global::System.Boolean operator ==({itemUnderlyingType} left, {structName} right) => Equals(left, right.Value);
        public static global::System.Boolean operator !=({itemUnderlyingType} left, {structName} right) => !Equals(left, right.Value);

        {GenerateComparableCode.GenerateIComparableImplementationIfNeeded(item, tds)}

        {GenerateCodeForTryParse.GenerateAnyHoistedTryParseMethods(item)}{GenerateCodeForParse.GenerateAnyHoistedParseMethods(item)}
        
        {GenerateEqualsAndHashCodes.GenerateGetHashCodeForAStruct(item)}

        {Util.GenerateToStringReadOnly(item)}

        private readonly void EnsureInitialized()
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
 
        {Util.GenerateAnyConversionBodies(tds, item)}

        {Util.GenerateDebuggerProxyForStructs(item)}

}}
{GenerateEfCoreExtensions.GenerateIfNeeded(item)}
{Util.WriteCloseNamespace(item.FullNamespace)}";
    }
}
