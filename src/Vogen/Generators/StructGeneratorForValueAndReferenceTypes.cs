using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators;

public class StructGeneratorForValueAndReferenceTypes : IGenerateSourceCode
{
    public string BuildClass(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var structName = tds.Identifier;
        return $@"using Vogen;

{Util.WriteStartNamespace(item.FullNamespace)}
    {Util.GenerateModifiersFor(tds)} struct {structName} : System.IEquatable<{structName}>
    {{
        private readonly {item.UnderlyingType} _value;

        public readonly {item.UnderlyingType} Value => _value;

        public {structName}()
        {{
            throw new Vogen.ValueObjectValidationException(""Validation skipped by attempting to use the default constructor. Please use the 'From' method for construction."");
        }}

        private {structName}({item.UnderlyingType} value) => _value = value;

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

        {Util.GenerateAnyInstances(tds, item)}
    }}
{Util.WriteCloseNamespace(item.FullNamespace)}";
    }
}