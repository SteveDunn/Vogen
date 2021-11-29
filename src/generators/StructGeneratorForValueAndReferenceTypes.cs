using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace generator.generators;

public class StructGeneratorForValueAndReferenceTypes : IGenerateSourceCode
{
    public string BuildClass(ValueObjectWorkItem item, TypeDeclarationSyntax tds, List<string> log)
    {
        var structName = tds.Identifier;
        return $@"using StringlyTyped.Core;

namespace {item.FullNamespace}
{{
    public partial struct {structName} : System.IEquatable<{structName}>
    {{
        // public readonly {item.UnderlyingType} Value {{ get; }}
        public readonly {item.UnderlyingType} Value;

        private {structName}({item.UnderlyingType} value) => Value = value;

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