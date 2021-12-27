using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators;

public class ClassGeneratorForValueType : IGenerateSourceCode
{
    public string BuildClass(VoWorkItem item, TypeDeclarationSyntax tds, List<string> log)
    {
        var className = tds.Identifier;
        return $@"
using Vogen;

namespace {item.FullNamespace}
{{
    {Util.GenerateModifiersFor(tds)} class {className} : System.IEquatable<{className}>
    {{
        public {className}()
        {{
            throw new Vogen.ValueObjectValidationException(""Validation skipped by attempting to use the default constructor. Please use the 'From' method for construction."");
        }}

        public {item.UnderlyingType} Value {{ get; }}

        private {className}({item.UnderlyingType} value)
        {{
            Value = value;
        }}

        /// <summary>
        /// Builds an instance from the provided underlying type.
        /// </summary>
        /// <param name=""value"">The underlying type.</param>
        /// <returns>An instance of this type.</returns>
        public static {className} From({item.UnderlyingType} value)
        {{ 
            {className} instance = new {className}(value);

            {Util.GenerateValidation(item)}

            return instance;
        }}

        public bool Equals({className} other)
        {{
            if (ReferenceEquals(null, other))
            {{
                return false;
            }}

            if (ReferenceEquals(this, other))
            {{
                return true;
            }}

            return GetType() == other.GetType() && System.Collections.Generic.EqualityComparer<{item.UnderlyingType}>.Default.Equals(Value, other.Value);
        }}

        public bool Equals({item.UnderlyingType} primitive) => Value.Equals(primitive);

        public override bool Equals(object obj)
        {{
            if (ReferenceEquals(null, obj))
            {{
                return false;
            }}

            if (ReferenceEquals(this, obj))
            {{
                return true;
            }}

            if (obj.GetType() != GetType())
            {{
                return false;
            }}

            return Equals(({className}) obj);
        }}

        public static bool operator ==({className} left, {className} right) => Equals(left, right);
        public static bool operator !=({className} left, {className} right) => !Equals(left, right);

        public static bool operator ==({className} left, {item.UnderlyingType} right) => Equals(left.Value, right);
        public static bool operator !=({className} left, {item.UnderlyingType} right) => !Equals(left.Value, right);

        public static bool operator ==({item.UnderlyingType} left, {className} right) => Equals(left, right.Value);
        public static bool operator !=({item.UnderlyingType} left, {className} right) => !Equals(left, right.Value);

        public override int GetHashCode()
        {{
            unchecked // Overflow is fine, just wrap
            {{
                int hash = (int) 2166136261;
                // Suitable nullity checks etc, of course :)
                hash = (hash * 16777619) ^ Value.GetHashCode();
                hash = (hash * 16777619) ^ GetType().GetHashCode();
                hash = (hash * 16777619) ^ System.Collections.Generic.EqualityComparer<{item.UnderlyingType}>.Default.GetHashCode();
                return hash;
            }}
        }}

        {Util.GenerateAnyInstances(tds, item, log)}

        public override string ToString() => Value.ToString();
    }}
}}";
    }
}