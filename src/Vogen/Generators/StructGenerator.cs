using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Generators.Conversions;

namespace Vogen.Generators;

public class StructGenerator : IGenerateValueObjectSourceCode
{
    public string Generate(GenerationParameters parameters)
    {
        var item = parameters.WorkItem;
        var tds = parameters.WorkItem.TypeToAugment;

        var structName = tds.Identifier;

        var itemUnderlyingType = item.UnderlyingTypeFullName;

        string underlyingNullAnnotation = item.Nullable.QuestionMarkForUnderlying;
        string underlyingBang = item.Nullable.BangForUnderlying;
        string wrapperBang = item.Nullable.BangForWrapper;

        var code = Generate();
        
        return item.Nullable.WrapBlock(code);
        
        string Generate() => $@"

{Util.WriteStartNamespace(item.FullNamespace)}
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] 
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{Util.GenerateYourAssemblyName()}"", ""{Util.GenerateYourAssemblyVersion()}"")]
    {Util.GenerateAnyConversionAttributes(tds, item)}
    {DebugGeneration.GenerateDebugAttributes(item, structName, itemUnderlyingType)}
// ReSharper disable once UnusedType.Global
    { Util.GenerateModifiersFor(tds)} struct {structName} : global::System.IEquatable<{structName}>{GenerateEqualsMethodsAndOperators.GenerateInterfaceIfNeeded(", ", itemUnderlyingType, item)}{GenerateComparableCode.GenerateIComparableHeaderIfNeeded(", ", item, tds)}{GenerateCodeForIParsableInterfaceDeclarations.GenerateIfNeeded(", ", item, tds)}{GenerateCodeForIFormattableInterfaceDeclarations.GenerateIfNeeded(", ", item, tds)}{WriteStaticAbstracts.WriteHeaderIfNeeded(", ", item, tds)}
    {{
{DebugGeneration.GenerateStackTraceFieldIfNeeded(item)}

#if !VOGEN_NO_VALIDATION
        private readonly global::System.Boolean _isInitialized;
#endif
        
        private readonly {itemUnderlyingType}{underlyingNullAnnotation} _value;

        {Util.GenerateCommentForValueProperty(item)}
        public readonly {itemUnderlyingType} Value
        {{
            [global::System.Diagnostics.DebuggerStepThroughAttribute]
            [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get
            {{
                EnsureInitialized();
                return _value{underlyingBang};
            }}
        }}

{GenerateStaticConstructor.GenerateIfNeeded(item)}
        [global::System.Diagnostics.DebuggerStepThroughAttribute]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        public {structName}()
        {{
#if DEBUG
            {DebugGeneration.SetStackTraceIfNeeded(item)}
#endif

#if !VOGEN_NO_VALIDATION
            _isInitialized = false;
#endif
            _value = default{wrapperBang};
        }}

        [global::System.Diagnostics.DebuggerStepThroughAttribute]
        private {structName}({itemUnderlyingType} value) 
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
        public static {structName} From({itemUnderlyingType} value)
        {{
            {Util.GenerateCallToNormalizeMethodIfNeeded(item)}

            {Util.GenerateCallToValidationAndThrowIfRequired(item)}

            return new {structName}(value);
        }}

        {GenerateCodeForTryFrom.GenerateForAStruct(item, structName, itemUnderlyingType)}

{Util.GenerateIsInitializedMethod(true, item)}

{GenerateStringComparers.GenerateIfNeeded(item, tds)}        

{GenerateCastingOperators.GenerateImplementations(item,tds)}{Util.GenerateGuidFactoryMethodIfNeeded(item)}
        // only called internally when something has been deserialized into
        // its primitive type.
        private static {structName} __Deserialize({itemUnderlyingType} value)
        {{
            {Util.GenerateCallToNormalizeMethodIfNeeded(item)}

            {Util.GenerateCallToValidateForDeserializing(item)}

            return new {structName}(value);
        }}
        {GenerateEqualsMethodsAndOperators.GenerateEqualsMethodsForAStruct(item, tds)}

        public static global::System.Boolean operator ==({structName} left, {structName} right) => left.Equals(right);
        public static global::System.Boolean operator !=({structName} left, {structName} right) => !(left == right);
        {GenerateEqualsMethodsAndOperators.GenerateEqualsOperatorsForPrimitivesIfNeeded(itemUnderlyingType, structName, item)}

        {GenerateComparableCode.GenerateIComparableImplementationIfNeeded(item, tds)}

        {GenerateCodeForTryParse.GenerateAnyHoistedTryParseMethods(item)}{GenerateCodeForParse.GenerateAnyHoistedParseMethods(item)}
        
        {GenerateCodeForTryFormat.GenerateAnyHoistedTryFormatMethods(parameters)}
        
        {GenerateHashCodes.GenerateForAStruct(item)}

        {GenerateCodeForToString.GenerateForAStruct(item)}

        {Util.GenerateEnsureInitializedMethod(item, readOnly: true)}

        {InstanceGeneration.GenerateAnyInstances(tds, item)}
 
        {Util.GenerateAnyConversionBodies(tds, item)}

        {Util.GenerateDebuggerProxyForStructs(item)}

        {Util.GenerateThrowHelper(item)}
}}
{GenerateEfCoreExtensions.GenerateInnerIfNeeded(item)}
{Util.WriteCloseNamespace(item.FullNamespace)}";
    }
}
