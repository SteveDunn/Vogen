using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators;

public class StructGeneratorForValueAndReferenceTypes : IGenerateSourceCode
{
    public string BuildClass(ValueObjectWorkItem item, TypeDeclarationSyntax tds, List<string> log)
    {
        var structName = tds.Identifier;
        return $@"using Vogen;

namespace {item.FullNamespace}
{{
    {Util.GenerateModifiersFor(tds)} struct {structName} : System.IEquatable<{structName}>
    {{
        private readonly bool _wasSet = false;

        private readonly {item.UnderlyingType} _value;

        public readonly {item.UnderlyingType} Value =>  _wasSet ? _value : throw new Vogen.ValueObjectValidationException(""Validation skipped by default initialisation. Please use the 'From' method for construction."");

        
        public {structName}()
        {{
            throw new Vogen.ValueObjectValidationException(""Validation skipped by attempting to use the default constructor. Please use the 'From' method for construction."");
        }}

        private {structName}({item.UnderlyingType} value)
        {{
            _wasSet = true;
            _value = value;
        }}

        public static {structName} From({item.UnderlyingType} value)
        {{
            {structName} instance = new {structName}(value);

            {Util.GenerateValidation(item)}
            return instance;
        }}

        public readonly bool Equals({structName} other)
        {{
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

        public readonly override int GetHashCode() => System.Collections.Generic.EqualityComparer<{item.UnderlyingType}>.Default.GetHashCode();

        public readonly override string ToString() => Value.ToString();

        {Util.GenerateAnyInstances(tds, item, log)}
    }}
}}";
    }
}