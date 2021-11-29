
using Vogen.SharedTypes;

namespace Vogen.Examples
{
    public partial class VendorId : System.IEquatable<VendorId>
    {
        public int Value { get; }

        private VendorId(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Builds an instance from the provided underlying type.
        /// </summary>
        /// <param name="value">The underlying type.</param>
        /// <returns>An instance of this type.</returns>
        public static VendorId From(int value)
        { 
            VendorId instance = new VendorId(value);

            var validation = VendorId.Validate(value);
            if (validation != Vogen.SharedTypes.Validation.Ok)
            {
                throw new Vogen.SharedTypes.ValueObjectValidationException(validation.ErrorMessage);
            }


            return instance;
        }

        public bool Equals(VendorId other)
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

            return Equals((VendorId) obj);
        }

        public static bool operator ==(VendorId left, VendorId right) => Equals(left, right);
        public static bool operator !=(VendorId left, VendorId right) => !Equals(left, right);

        public static bool operator ==(VendorId left, int right) => Equals(left.Value, right);
        public static bool operator !=(VendorId left, int right) => !Equals(left.Value, right);

        public static bool operator ==(int left, VendorId right) => Equals(left, right.Value);
        public static bool operator !=(int left, VendorId right) => !Equals(left, right.Value);

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

        
// instance...

public static VendorId Unspecified = new VendorId(0);

// instance...

public static VendorId Invalid = new VendorId(-1);


        public override string ToString() => Value.ToString();
    }
}