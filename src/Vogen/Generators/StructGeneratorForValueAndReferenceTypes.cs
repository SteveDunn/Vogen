using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators;

public class StructGeneratorForValueAndReferenceTypes : IGenerateSourceCode
{
    public string BuildClass(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var structName = tds.Identifier;
        return $@"using Vogen;

{Util.WriteStartNamespace(item.FullNamespace)}
    {Util.GenerateAnyConversionAttributes(tds, item)}
    {Util.GenerateModifiersFor(tds)} struct {structName} : System.IEquatable<{structName}>
    {{
#if DEBUG    
        private readonly System.Diagnostics.StackTrace _stackTrace = null;
#endif

        private readonly bool _isInitialized;
        
        private readonly {item.UnderlyingType} _value;

        public readonly {item.UnderlyingType} Value
        {{
            [System.Diagnostics.DebuggerStepThroughAttribute]
            get
            {{
                EnsureInitialized();
                return _value;
            }}
        }}

        [System.Diagnostics.DebuggerStepThroughAttribute]
        public {structName}()
        {{
#if DEBUG
            _stackTrace = new System.Diagnostics.StackTrace();
#endif

            _isInitialized = false;
            _value = default;
        }}

        [System.Diagnostics.DebuggerStepThroughAttribute]
        private {structName}({item.UnderlyingType} value) 
        {{
            _value = value;
            _isInitialized = true;
        }}

        public static {structName} From({item.UnderlyingType} value)
        {{
            {structName} instance = new {structName}(value);

            {Util.GenerateValidation(item)}

            return instance;
        }}

        public readonly bool Equals({structName} other)
        {{
            // It's possible to create uninitialized instances via converters such as EfCore (HasDefaultValue), which call Equals.
            // We treat anything uninitialized as not equal to anything, even other uninitialized instances of this type.
            if(!_isInitialized || !other._isInitialized) return false;

            return System.Collections.Generic.EqualityComparer<{item.UnderlyingType}>.Default.Equals(Value, other.Value);
        }}

        public readonly bool Equals({item.UnderlyingType} primitive) => Value.Equals(primitive);

        public readonly override bool Equals(object obj)
        {{
            return obj is {structName} && Equals(({structName}) obj);
        }}

        public static bool operator ==({structName} left, {structName} right) => Equals(left, right);
        public static bool operator !=({structName} left, {structName} right) => !(left == right);

        public static bool operator ==({structName} left, {item.UnderlyingType} right) => Equals(left.Value, right);
        public static bool operator !=({structName} left, {item.UnderlyingType} right) => !Equals(left.Value, right);

        public static bool operator ==({item.UnderlyingType} left, {structName} right) => Equals(left, right.Value);
        public static bool operator !=({item.UnderlyingType} left, {structName} right) => !Equals(left, right.Value);

        public readonly override int GetHashCode() => System.Collections.Generic.EqualityComparer<{item.UnderlyingType}>.Default.GetHashCode(_value);

        public readonly override string ToString() => Value.ToString();

        private readonly void EnsureInitialized()
        {{
            if (!_isInitialized)
            {{
#if DEBUG
                string message = ""Use of uninitialized Value Object at: "" + _stackTrace ?? """";
#else
                string message = ""Use of uninitialized Value Object."";
#endif

                throw new ValueObjectValidationException(message);
            }}
        }}

        { Util.GenerateAnyInstances(tds, item)}
 
        {Util.GenerateAnyConversionBodies(tds, item)}

}}
{Util.WriteCloseNamespace(item.FullNamespace)}";
    }
}