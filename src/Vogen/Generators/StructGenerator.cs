using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators;

public class StructGenerator : IGenerateSourceCode
{
    public string BuildClass(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var structName = tds.Identifier;
        var itemUnderlyingType = item.UnderlyingType?.ToString() ?? "global::System.Int32";

        return $@"using Vogen;

{Util.WriteStartNamespace(item.FullNamespace)}
    {Util.GenerateAnyConversionAttributes(tds, item)}
    [global::System.Diagnostics.DebuggerTypeProxyAttribute(typeof({structName}DebugView))]
    [global::System.Diagnostics.DebuggerDisplayAttribute(""Underlying type: {itemUnderlyingType}, Value = {{ _value }}"")]
    { Util.GenerateModifiersFor(tds)} struct {structName} : global::System.IEquatable<{structName}>
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
            {structName} instance = new {structName}(value);

            {Util.GenerateValidation(item)}

            return instance;
        }}

        public readonly global::System.Boolean Equals({structName} other)
        {{
            // It's possible to create uninitialized instances via converters such as EfCore (HasDefaultValue), which call Equals.
            // We treat anything uninitialized as not equal to anything, even other uninitialized instances of this type.
            if(!_isInitialized || !other._isInitialized) return false;

            return global::System.Collections.Generic.EqualityComparer<{itemUnderlyingType}>.Default.Equals(Value, other.Value);
        }}

        public readonly global::System.Boolean Equals({itemUnderlyingType} primitive) => Value.Equals(primitive);

        public readonly override global::System.Boolean Equals(global::System.Object obj)
        {{
            return obj is {structName} && Equals(({structName}) obj);
        }}

        public static global::System.Boolean operator ==({structName} left, {structName} right) => Equals(left, right);
        public static global::System.Boolean operator !=({structName} left, {structName} right) => !(left == right);

        public static global::System.Boolean operator ==({structName} left, {itemUnderlyingType} right) => Equals(left.Value, right);
        public static global::System.Boolean operator !=({structName} left, {itemUnderlyingType} right) => !Equals(left.Value, right);

        public static global::System.Boolean operator ==({itemUnderlyingType} left, {structName} right) => Equals(left, right.Value);
        public static global::System.Boolean operator !=({itemUnderlyingType} left, {structName} right) => !Equals(left, right.Value);

        public readonly override global::System.Int32 GetHashCode() => global::System.Collections.Generic.EqualityComparer<{itemUnderlyingType}>.Default.GetHashCode(_value);

        public readonly override global::System.String ToString() => Value.ToString();

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

        { Util.GenerateAnyInstances(tds, item)}
 
        {Util.GenerateAnyConversionBodies(tds, item)}

        {Util.GenerateDebuggerProxyForStructs(tds, item)}

}}
{Util.WriteCloseNamespace(item.FullNamespace)}";
    }
}
