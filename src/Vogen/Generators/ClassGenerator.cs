using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vogen.Generators.Conversions;

namespace Vogen.Generators;

public class ClassGenerator : IGenerateSourceCode
{
    public string BuildClass(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        SyntaxToken className = tds.Identifier;

        string itemUnderlyingType = item.UnderlyingTypeFullName;

        string wrapperQ = item.Nullable.QuestionMarkForWrapper;
        string underlyingQ = item.Nullable.QuestionMarkForUnderlying;
        string underlyingBang = item.Nullable.BangForUnderlying;
        string wrapperBang = item.Nullable.BangForWrapper;

        var code = Generate();

        return item.Nullable.WrapBlock(code);
    
    
    string Generate() => $@"
using Vogen;

{Util.WriteStartNamespace(item.FullNamespace)}
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] 
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{Util.GenerateYourAssemblyName()}"", ""{Util.GenerateYourAssemblyVersion()}"")]
    {Util.GenerateAnyConversionAttributes(tds, item)}
    {DebugGeneration.GenerateDebugAttributes(item, className, itemUnderlyingType)}
    {Util.GenerateModifiersFor(tds)} class {className} : global::System.IEquatable<{className}>{GenerateEqualsMethodsAndOperators.GenerateInterfaceIfNeeded(", ", itemUnderlyingType, item)}{GenerateComparableCode.GenerateIComparableHeaderIfNeeded(", ", item, tds)}{GenerateCodeForIParsableInterfaceDeclarations.GenerateIfNeeded(", ", item, tds)}{WriteStaticAbstracts.WriteHeaderIfNeeded(", ", item, tds)}
    {{
        {DebugGeneration.GenerateStackTraceFieldIfNeeded(item)}
#if !VOGEN_NO_VALIDATION
        private readonly global::System.Boolean _isInitialized;
#endif
        private readonly {itemUnderlyingType}{underlyingQ} _value;
        
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
        }}

{GenerateStaticConstructor.GenerateIfNeeded(item)}
        [global::System.Diagnostics.DebuggerStepThroughAttribute]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        public {className}()
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
        private {className}({itemUnderlyingType} value)
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
        public static {className} From({itemUnderlyingType} value)
        {{
            {GenerateNullCheckAndThrowIfNeeded(item)}

            {Util.GenerateCallToNormalizeMethodIfNeeded(item)}

            {Util.GenerateCallToValidationAndThrowIfRequired(item)}

            return new {className}(value);
        }}

        {GenerateCodeForTryFrom.GenerateForAClass(item, className, itemUnderlyingType)}

        {Util.GenerateIsInitializedMethod(false, item)}

        {GenerateStringComparers.GenerateIfNeeded(item, tds)}  

        // only called internally when something has been deserialized into
        // its primitive type.
        private static {className} __Deserialize({itemUnderlyingType} value)
        {{
            {GenerateNullCheckAndThrowIfNeeded(item)}

            {Util.GenerateCallToNormalizeMethodIfNeeded(item)}

            {Util.GenerateCallToValidateForDeserializing(item)}

            return new {className}(value);
        }}
        {GenerateEqualsMethodsAndOperators.GenerateEqualsMethodsForAClass(item, tds)}

        public static global::System.Boolean operator ==({className}{wrapperQ} left, {className}{wrapperQ} right) => Equals(left, right);
        public static global::System.Boolean operator !=({className}{wrapperQ} left, {className}{wrapperQ} right) => !Equals(left, right);
        {GenerateEqualsMethodsAndOperators.GenerateEqualsOperatorsForPrimitivesIfNeeded(itemUnderlyingType, className, item)}

        {GenerateCastingOperators.GenerateImplementations(item,tds)}{Util.GenerateGuidFactoryMethodIfNeeded(item)}
        {GenerateComparableCode.GenerateIComparableImplementationIfNeeded(item, tds)}

        {GenerateCodeForTryParse.GenerateAnyHoistedTryParseMethods(item)}{GenerateCodeForParse.GenerateAnyHoistedParseMethods(item)}

        {GenerateHashCodes.GenerateGetHashCodeForAClass(item)}

        {Util.GenerateEnsureInitializedMethod(item, readOnly: false)}

        {InstanceGeneration.GenerateAnyInstances(tds, item)}

        {Util.GenerateToString(item)}

        {Util.GenerateAnyConversionBodies(tds, item)}

        {Util.GenerateDebuggerProxyForClasses(tds, item)}

        {Util.GenerateThrowHelper(item)}
    }}
{GenerateEfCoreExtensions.GenerateInnerIfNeeded(item)}
{Util.WriteCloseNamespace(item.FullNamespace)}
";
    }

    private static string GenerateNullCheckAndThrowIfNeeded(VoWorkItem voWorkItem) =>
        voWorkItem.IsTheUnderlyingAValueType ? string.Empty
            : """
                  if (value is null)
                  {
                      ThrowHelper.ThrowWhenCreatedWithNull();
                      return default!;
                  }

              """;
}

