using Vogen;

namespace Vogen.Examples
{
    public partial struct MyReadOnlyVo : System.IEquatable<MyReadOnlyVo>
    {
        // public readonly int Value { get; }
        public readonly int Value;

        private MyReadOnlyVo(int value) => Value = value;

        public static MyReadOnlyVo From(int value)
        {
            MyReadOnlyVo instance = new MyReadOnlyVo(value);

            

            return instance;
        }

        public readonly bool Equals(MyReadOnlyVo other)
        {
            return System.Collections.Generic.EqualityComparer<int>.Default.Equals(Value, other.Value);
        }

        public readonly bool Equals(int primitive) => Value.Equals(primitive);

        public readonly override bool Equals(object obj)
        {
            return obj is MyReadOnlyVo && Equals((MyReadOnlyVo) obj);
        }

        public static bool operator ==(MyReadOnlyVo left, MyReadOnlyVo right) => Equals(left, right);
        public static bool operator !=(MyReadOnlyVo left, MyReadOnlyVo right) => !(left == right);

        public static bool operator ==(MyReadOnlyVo left, int right) => Equals(left.Value, right);
        public static bool operator !=(MyReadOnlyVo left, int right) => !Equals(left.Value, right);

        public static bool operator ==(int left, MyReadOnlyVo right) => Equals(left, right.Value);
        public static bool operator !=(int left, MyReadOnlyVo right) => !Equals(left, right.Value);

        public readonly override int GetHashCode() => System.Collections.Generic.EqualityComparer<int>.Default.GetHashCode();

        public readonly override string ToString() => Value.ToString();

        
    }
}