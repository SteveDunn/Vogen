using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Generators.Conversions;

namespace Vogen.Generators;

public class RecordStructGenerator : IGenerateSourceCode
{
    public string BuildClass(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var wrapperName = tds.Identifier;

        var itemUnderlyingType = item.UnderlyingTypeFullName;

        string qmForUnderlying = item.Nullable.QuestionMarkForUnderlying;
        string bangForUnderlying = item.Nullable.BangForUnderlying;
        
        var code = Generate();
        
        return item.Nullable.WrapBlock(code);
        
        string Generate() => $@"
using Vogen;

{Util.WriteStartNamespace(item.FullNamespace)}
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] 
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{Util.GenerateYourAssemblyName()}"", ""{Util.GenerateYourAssemblyVersion()}"")]
    {Util.GenerateAnyConversionAttributes(tds, item)}
    {DebugGeneration.GenerateDebugAttributes(item, wrapperName, itemUnderlyingType)}
    { Util.GenerateModifiersFor(tds)} record struct {wrapperName} : global::System.IEquatable<{wrapperName}>{GenerateEqualsMethodsAndOperators.GenerateInterfaceIfNeeded(", ", itemUnderlyingType, item)}{GenerateComparableCode.GenerateIComparableHeaderIfNeeded(", ", item, tds)}{GenerateCodeForIParsableInterfaceDeclarations.GenerateIfNeeded(", ", item, tds)}{WriteStaticAbstracts.WriteHeaderIfNeeded(", ", item, tds)}
    {{
{DebugGeneration.GenerateStackTraceFieldIfNeeded(item)}

#if !VOGEN_NO_VALIDATION
        private readonly global::System.Boolean _isInitialized;
#endif
        
        private readonly {itemUnderlyingType}{qmForUnderlying} _value;

        {Util.GenerateCommentForValueProperty(item)}
        public readonly {itemUnderlyingType} Value
        {{
            [global::System.Diagnostics.DebuggerStepThroughAttribute]
            get
            {{
                EnsureInitialized();
                return _value{bangForUnderlying};
            }}
            [global::System.Diagnostics.DebuggerStepThroughAttribute]
            init
            {{
                {GenerateNullCheckIfNeeded(item)}

                {Util.GenerateCallToNormalizeMethodIfNeeded(item)}

                {Util.GenerateCallToValidationAndThrowIfRequired(item)}

                _value = value;
            }}
        }}

{GenerateStaticConstructor.GenerateIfNeeded(item)}
        [global::System.Diagnostics.DebuggerStepThroughAttribute]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        public {wrapperName}()
        {{
#if DEBUG
            {DebugGeneration.SetStackTraceIfNeeded(item)}
#endif

#if !VOGEN_NO_VALIDATION
            _isInitialized = false;
#endif
            _value = default;
        }}

        [global::System.Diagnostics.DebuggerStepThroughAttribute]
        private {wrapperName}({itemUnderlyingType} value) 
        {{
            _value = value;
#if !VOGEN_NO_VALIDATION
            _isInitialized = true;
#endif
        }}

        /// <summary>
        /// Builds an instance from the provided underlying type.
        /// </summary>
        /// <param name=""value"">The underlying type.</param>
        /// <returns>An instance of this type.</returns>
        public static {wrapperName} From({itemUnderlyingType} value)
        {{
            {Util.GenerateCallToNormalizeMethodIfNeeded(item)}

            {Util.GenerateCallToValidationAndThrowIfRequired(item)}

            {wrapperName} instance = new {wrapperName}(value);

            return instance;
        }}

        {GenerateCodeForTryFrom.GenerateForAStruct(item, wrapperName, itemUnderlyingType)}

{Util.GenerateIsInitializedMethod(true, item)}

{GenerateStringComparers.GenerateIfNeeded(item, tds)}        
{GenerateCastingOperators.GenerateImplementations(item,tds)}{Util.GenerateGuidFactoryMethodIfNeeded(item)}
        // only called internally when something has been deserialized into
        // its primitive type.
        private static {wrapperName} __Deserialize({itemUnderlyingType} value)
        {{
            {Util.GenerateCallToNormalizeMethodIfNeeded(item)}

            {Util.GenerateCallToValidateForDeserializing(item)}

            return new {wrapperName}(value);
        }}
        {GenerateEqualsMethodsAndOperators.GenerateEqualsMethodsForAStruct(item, tds)}
{GenerateEqualsMethodsAndOperators.GenerateEqualsOperatorsForPrimitivesIfNeeded(itemUnderlyingType, wrapperName, item)}

        {GenerateComparableCode.GenerateIComparableImplementationIfNeeded(item, tds)}

        {GenerateCodeForTryParse.GenerateAnyHoistedTryParseMethods(item)}{GenerateCodeForParse.GenerateAnyHoistedParseMethods(item)}

        {GenerateHashCodes.GenerateForAStruct(item)}

#if NETCOREAPP3_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.MemberNotNullAttribute(nameof(_value))]
        [global::System.Diagnostics.CodeAnalysis.MemberNotNullAttribute(nameof(Value))]
#endif
        private readonly void EnsureInitialized()
        {{
            if (!IsInitialized())
            {{
#if DEBUG
                {DebugGeneration.GenerateMessageForUninitializedValueObject(item)}
#else
                global::System.String message = ""Use of uninitialized Value Object."";
#endif

                throw new {item.ValidationExceptionFullName}(message);
            }}
        }}

        // record enumerates fields - we just want our Value and to throw if it's not initialized.
        {Util.GenerateToStringReadOnly(item)}

        {InstanceGeneration.GenerateAnyInstances(tds, item)}
 
        {Util.GenerateAnyConversionBodies(tds, item)}

        {Util.GenerateDebuggerProxyForStructs(item)}

}}
{GenerateEfCoreExtensions.GenerateInnerIfNeeded(item)}
{Util.WriteCloseNamespace(item.FullNamespace)}";
    }

    private static string GenerateNullCheckIfNeeded(VoWorkItem voWorkItem) =>
        voWorkItem.IsTheUnderlyingAValueType ? string.Empty
            : $$"""
                    if (value is null)
                    {
                        throw new {{voWorkItem.ValidationExceptionFullName}}("Cannot create a value object with null.");
                    }

                """;
}