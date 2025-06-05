using Microsoft.CodeAnalysis;
using Vogen.Generators.Conversions;

namespace Vogen.Generators;

public class ClassGenerator : IGenerateValueObjectSourceCode
{
    public string Generate(GenerationParameters parameters)
    {
        var item = parameters.WorkItem;
        var tds = parameters.WorkItem.TypeToAugment;
        
        SyntaxToken wrapperName = tds.Identifier;

        string itemUnderlyingType = item.UnderlyingTypeFullName;

        string wrapperQ = item.Nullable.QuestionMarkForWrapper;
        string underlyingNullAnnotation = item.Nullable.QuestionMarkForUnderlying;
        string underlyingBang = item.Nullable.BangForUnderlying;
        
        string readonlyForValueAndIsInitialized = Util.GetModifiersForValueAndIsInitializedFields(parameters.WorkItem); 

        var code = GenerateCode();

        return item.Nullable.WrapBlock(code);
    
    string GenerateCode() => $@"

{Util.WriteStartNamespace(item.FullNamespace)}
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] 
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{Util.GenerateYourAssemblyName()}"", ""{Util.GenerateYourAssemblyVersion()}"")]
    {Util.GenerateAnyConversionAttributes(tds, item)}
    {DebugGeneration.GenerateDebugAttributes(item, wrapperName, itemUnderlyingType)}
    {Util.GenerateModifiersFor(tds)} class {wrapperName} : 
        global::System.IEquatable<{wrapperName}>
        {GenerateCodeForEqualsMethodsAndOperators.GenerateInterfaceDefinitionsIfNeeded(", ", item)}
        {GenerateCodeForComparables.GenerateInterfaceDefinitionsIfNeeded(", ", item)}
        {GenerateCodeForIParsableInterfaceDeclarations.GenerateIfNeeded(", ", item)}
        {GenerateCodeForTryFormat.GenerateInterfaceDefinitionsIfNeeded(", ", parameters)}
        {GenerateCodeForStaticAbstracts.GenerateInterfaceDefinitionIfNeeded(", ", item)}
        {GenerateCodeForXmlSerializable.GenerateInterfaceDefinitionIfNeeded(", ", item)}
    {{
        {DebugGeneration.GenerateStackTraceFieldIfNeeded(item)}

#if !VOGEN_NO_VALIDATION
        private {readonlyForValueAndIsInitialized} global::System.Boolean _isInitialized;
#endif
        
        private {readonlyForValueAndIsInitialized} {itemUnderlyingType}{underlyingNullAnnotation} _value;
        
        {Util.GenerateCommentForValueProperty(item)}
        public {itemUnderlyingType} Value
        {{
            [global::System.Diagnostics.DebuggerStepThroughAttribute]
            [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get
            {{
                EnsureInitialized();
                return _value{underlyingBang};
            }}
        }}

{GenerateCodeForStaticConstructors.GenerateIfNeeded(item)}
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

            {GenerateCodeForCallingValidation.CallAndThrowIfRequired(item)}

            return new {wrapperName}(value);
        }}

        {GenerateCodeForTryFrom.GenerateForAClass(item, wrapperName, itemUnderlyingType)}

        {Util.GenerateIsInitializedMethod(false, item)}

        {Util.GenerateLinqPadDump(item)}

        {GenerateCodeForStringComparers.GenerateIfNeeded(item, tds)}  

        // only called internally when something has been deserialized into
        // its primitive type.
        private static {wrapperName} __Deserialize({itemUnderlyingType} value)
        {{
            {GenerateNullCheckAndThrowIfNeeded(item)}

            {Util.GenerateCallToNormalizeMethodIfNeeded(item)}

            {Util.GenerateChecksForKnownInstancesIfRequired(item)}

            {GenerateCodeForCallingValidation.CallWhenDeserializingAndCheckStrictnessFlag(item)}

            return new {wrapperName}(value);
        }}
        {GenerateCodeForEqualsMethodsAndOperators.GenerateEqualsMethodsForAClass(item, tds)}

        public static global::System.Boolean operator ==({wrapperName}{wrapperQ} left, {wrapperName}{wrapperQ} right) => Equals(left, right);
        public static global::System.Boolean operator !=({wrapperName}{wrapperQ} left, {wrapperName}{wrapperQ} right) => !Equals(left, right);
        {GenerateCodeForEqualsMethodsAndOperators.GenerateEqualsOperatorsForPrimitivesIfNeeded(itemUnderlyingType, wrapperName, item)}

        {GenerateCodeForCastingOperators.GenerateImplementations(parameters,tds)}{Util.GenerateGuidFactoryMethodIfNeeded(item)}
        {GenerateCodeForComparables.GenerateIComparableImplementationIfNeeded(item, tds)}

        {GenerateCodeForTryParse.GenerateAnyHoistedTryParseMethods(item)}{GenerateCodeForParse.GenerateAnyHoistedParseMethods(item)}

        {GenerateCodeForTryFormat.GenerateAnyHoistedTryFormatMethods(parameters)}

        {GenerateCodeForHashCodes.GenerateGetHashCodeForAClass(item)}

        {GenerateCodeForToString.GenerateForAClass(parameters)}

        {Util.GenerateEnsureInitializedMethod(item, readOnly: false)}

        {InstanceGeneration.GenerateAnyInstances(tds, item)}

        {Util.GenerateAnyConversionBodies(tds, item)}

        {GenerateCodeForXmlSerializable.GenerateBodyIfNeeded(parameters)}

        {Util.GenerateDebuggerProxyForClasses(tds, item)}

        {Util.GenerateThrowHelper(item)}
    }}

{GenerateEfCoreExtensions.GenerateInnerIfNeeded(item)}
{Util.WriteCloseNamespace(item.FullNamespace)}";
    }

    private static string GenerateNullCheckAndThrowIfNeeded(VoWorkItem voWorkItem) =>
        voWorkItem.IsTheUnderlyingAValueType ? string.Empty
            : """
                  if (value is null)
                  {
                      ThrowHelper.ThrowWhenCreatedWithNull();
                  }

              """;
}