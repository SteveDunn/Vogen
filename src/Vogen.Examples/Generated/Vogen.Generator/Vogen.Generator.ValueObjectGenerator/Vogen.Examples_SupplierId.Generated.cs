
using Vogen;

namespace Vogen.Examples
{
    public partial class SupplierId : System.IEquatable<SupplierId>
    {
        public int Value { get; }

        private SupplierId(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Builds an instance from the provided underlying type.
        /// </summary>
        /// <param name="value">The underlying type.</param>
        /// <returns>An instance of this type.</returns>
        public static SupplierId From(int value)
        { 
            SupplierId instance = new SupplierId(value);

            

            return instance;
        }

        public bool Equals(SupplierId other)
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

            return Equals((SupplierId) obj);
        }

        public static bool operator ==(SupplierId left, SupplierId right) => Equals(left, right);
        public static bool operator !=(SupplierId left, SupplierId right) => !Equals(left, right);

        public static bool operator ==(SupplierId left, int right) => Equals(left.Value, right);
        public static bool operator !=(SupplierId left, int right) => !Equals(left.Value, right);

        public static bool operator ==(int left, SupplierId right) => Equals(left, right.Value);
        public static bool operator !=(int left, SupplierId right) => !Equals(left, right.Value);

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