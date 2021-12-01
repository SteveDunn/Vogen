
using Vogen;

namespace Vogen.Examples
{
    public partial class VendorName : System.IEquatable<VendorName>
    {
        public string Value { get; }

        private VendorName(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Builds an instance from the provided underlying type.
        /// </summary>
        /// <param name="value">The underlying type.</param>
        /// <returns>An instance of this type.</returns>
        public static VendorName From(string value)
        {
            if (value is null)
            {
                throw new Vogen.ValueObjectValidationException("Cannot create a value object with null.");
            }

            VendorName instance = new VendorName(value);

            

            return instance;
        }

        public bool Equals(VendorName other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return GetType() == other.GetType() && System.Collections.Generic.EqualityComparer<string>.Default.Equals(Value, other.Value);
        }

        public bool Equals(string primitive) => Value.Equals(primitive);

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

            return Equals((VendorName) obj);
        }

        public static bool operator ==(VendorName left, VendorName right) => Equals(left, right);
        public static bool operator !=(VendorName left, VendorName right) => !Equals(left, right);

        public static bool operator ==(VendorName left, string right) => Equals(left.Value, right);
        public static bool operator !=(VendorName left, string right) => !Equals(left.Value, right);

        public static bool operator ==(string left, VendorName right) => Equals(left, right.Value);
        public static bool operator !=(string left, VendorName right) => !Equals(left, right.Value);

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = (int) 2166136261;
                // Suitable nullity checks etc, of course :)
                hash = (hash * 16777619) ^ Value.GetHashCode();
                hash = (hash * 16777619) ^ GetType().GetHashCode();
                hash = (hash * 16777619) ^ System.Collections.Generic.EqualityComparer<string>.Default.GetHashCode();
                return hash;
            }
        }

        
// instance...

public static VendorName Invalid = new VendorName("[INVALID]");


        public override string ToString() => Value.ToString();
    }
}