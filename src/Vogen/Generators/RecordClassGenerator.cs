using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Generators.Conversions;

namespace Vogen.Generators;

public class RecordClassGenerator : IGenerateSourceCode
{
    public string BuildClass(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var wrapperName = tds.Identifier;

        var itemUnderlyingType = item.UnderlyingTypeFullName;

        string underlyingNullAnnotation = item.Nullable.QuestionMarkForUnderlying;
        string underlyingBang = item.Nullable.BangForUnderlying;
        
        var code = Generate();
        
        return item.Nullable.WrapBlock(code);
        
        string Generate() => $@"

{Util.WriteStartNamespace(item.FullNamespace)}
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] 
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{Util.GenerateYourAssemblyName()}"", ""{Util.GenerateYourAssemblyVersion()}"")]
    {Util.GenerateAnyConversionAttributes(tds, item)}
    {DebugGeneration.GenerateDebugAttributes(item, wrapperName, itemUnderlyingType)}
    {Util.GenerateModifiersFor(tds)} record class {wrapperName} : global::System.IEquatable<{wrapperName}>{GenerateEqualsMethodsAndOperators.GenerateInterfaceIfNeeded(", ", itemUnderlyingType, item)}{GenerateComparableCode.GenerateIComparableHeaderIfNeeded(", ", item, tds)}{GenerateCodeForIParsableInterfaceDeclarations.GenerateIfNeeded(", ", item, tds)}{WriteStaticAbstracts.WriteHeaderIfNeeded(", ", item, tds)}
    {{
{DebugGeneration.GenerateStackTraceFieldIfNeeded(item)}
#if !VOGEN_NO_VALIDATION
        private readonly global::System.Boolean _isInitialized;
#endif
        private readonly {itemUnderlyingType}{underlyingNullAnnotation} _value;
        
        {Util.GenerateCommentForValueProperty(item)}
        public {itemUnderlyingType} Value
        {{
            [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            [global::System.Diagnostics.DebuggerStepThroughAttribute]
            get
            {{
                EnsureInitialized();
                return _value{underlyingBang};
            }}
            [global::System.Diagnostics.DebuggerStepThroughAttribute]
            init
            {{
                {GenerateNullCheckAndThrowIfNeeded(item)}

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
            _value = default{underlyingBang};
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
        [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static {wrapperName} From({itemUnderlyingType} value)
        {{
            {GenerateNullCheckAndThrowIfNeeded(item)}

            {Util.GenerateCallToNormalizeMethodIfNeeded(item)}

            {Util.GenerateCallToValidationAndThrowIfRequired(item)}

            return new {wrapperName}(value);
        }}

        {GenerateCodeForTryFrom.GenerateForAStruct(item, wrapperName, itemUnderlyingType)}

{Util.GenerateIsInitializedMethod(false, item)}

{GenerateStringComparers.GenerateIfNeeded(item, tds)}  
        // only called internally when something has been deserialized into
        // its primitive type.
        private static {wrapperName} __Deserialize({itemUnderlyingType} value)
        {{
            {GenerateNullCheckAndThrowIfNeeded(item)}

            {Util.GenerateCallToNormalizeMethodIfNeeded(item)}

            {Util.GenerateCallToValidateForDeserializing(item)}

            return new {wrapperName}(value);
        }}
        {GenerateEqualsMethodsAndOperators.GenerateEqualsMethodsForAClass(item, tds)}
{GenerateEqualsMethodsAndOperators.GenerateEqualsOperatorsForPrimitivesIfNeeded(itemUnderlyingType, wrapperName, item)}

{GenerateCastingOperators.GenerateImplementations(item,tds)}{Util.GenerateGuidFactoryMethodIfNeeded(item)}
        {GenerateComparableCode.GenerateIComparableImplementationIfNeeded(item, tds)}

        {GenerateCodeForTryParse.GenerateAnyHoistedTryParseMethods(item)}{GenerateCodeForParse.GenerateAnyHoistedParseMethods(item)}

        {GenerateHashCodes.GenerateGetHashCodeForAClass(item)}

        {Util.GenerateEnsureInitializedMethod(item, readOnly: false)}

        {InstanceGeneration.GenerateAnyInstances(tds, item)}

        // record enumerates fields - we just want our Value and to throw if it's not initialized.
        {GenerateCodeForToString.GenerateForAClass(item)}

        {Util.GenerateAnyConversionBodies(tds, item)}

        {Util.GenerateDebuggerProxyForClasses(tds, item)}

        {Util.GenerateThrowHelper(item)}
   }}
{GenerateEfCoreExtensions.GenerateInnerIfNeeded(item)}
{Util.WriteCloseNamespace(item.FullNamespace)}";
    }

    private static string GenerateNullCheckAndThrowIfNeeded(VoWorkItem voWorkItem) =>
        voWorkItem.IsTheUnderlyingAValueType ? string.Empty
            : $$"""
                    if (value is null)
                    {
                        throw new {{voWorkItem.ValidationExceptionFullName}}("Cannot create a value object with null.");
                    }

                """;
}