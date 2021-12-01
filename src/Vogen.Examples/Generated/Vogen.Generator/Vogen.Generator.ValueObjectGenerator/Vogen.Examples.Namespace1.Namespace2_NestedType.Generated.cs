using Vogen;

namespace Vogen.Examples.Namespace1.Namespace2
{
    public partial struct NestedType : System.IEquatable<NestedType>
    {
        // public readonly int Value { get; }
        public readonly int Value;

        private NestedType(int value) => Value = value;

        public static NestedType From(int value)
        {
            NestedType instance = new NestedType(value);

            

            return instance;
        }

        public readonly bool Equals(NestedType other)
        {
            return System.Collections.Generic.EqualityComparer<int>.Default.Equals(Value, other.Value);
        }

        public readonly bool Equals(int primitive) => Value.Equals(primitive);

        public readonly override bool Equals(object obj)
        {
            return obj is NestedType && Equals((NestedType) obj);
        }

        public static bool operator ==(NestedType left, NestedType right) => Equals(left, right);
        public static bool operator !=(NestedType left, NestedType right) => !(left == right);

        public static bool operator ==(NestedType left, int right) => Equals(left.Value, right);
        public static bool operator !=(NestedType left, int right) => !Equals(left.Value, right);

        public static bool operator ==(int left, NestedType right) => Equals(left, right.Value);
        public static bool operator !=(int left, NestedType right) => !Equals(left, right.Value);

        public readonly override int GetHashCode() => System.Collections.Generic.EqualityComparer<int>.Default.GetHashCode();

        public readonly override string ToString() => Value.ToString();

        
    }
}