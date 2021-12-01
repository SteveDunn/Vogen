
using Vogen.SharedTypes;

namespace Vogen.Examples
{
    public partial class Amount : System.IEquatable<Amount>
    {
        public int Value { get; }

        private Amount(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Builds an instance from the provided underlying type.
        /// </summary>
        /// <param name="value">The underlying type.</param>
        /// <returns>An instance of this type.</returns>
        public static Amount From(int value)
        { 
            Amount instance = new Amount(value);

            

            return instance;
        }

        public bool Equals(Amount other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return GetType() == other.GetType() && System.Collections.Generic.EqualityComparer<int>.Default.Equals(Value, other.Value);
        }

        public bool Equals(int primitive) => Value.Equals(primitive);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Amount) obj);
        }

        public static bool operator ==(Amount left, Amount right) => Equals(left, right);
        public static bool operator !=(Amount left, Amount right) => !Equals(left, right);

        public static bool operator ==(Amount left, int right) => Equals(left.Value, right);
        public static bool operator !=(Amount left, int right) => !Equals(left.Value, right);

        public static bool operator ==(int left, Amount right) => Equals(left, right.Value);
        public static bool operator !=(int left, Amount right) => !Equals(left, right.Value);

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = (int) 2166136261;
                // Suitable nullity checks etc, of course :)
                hash = (hash * 16777619) ^ Value.GetHashCode();
                hash = (hash * 16777619) ^ GetType().GetHashCode();
                hash = (hash * 16777619) ^ System.Collections.Generic.EqualityComparer<int>.Default.GetHashCode();
                return hash;
            }
        }

        

        public override string ToString() => Value.ToString();
    }
}