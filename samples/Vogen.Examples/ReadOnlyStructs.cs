using Vogen;

namespace Vogen.Examples
{
    [ValueObject(typeof(int))]
    public readonly partial struct MyReadOnlyVo
    {
    }

    public class UseReadOnly
    {
        public UseReadOnly()
        {
            MyReadOnlyVo.From(123);
        }
    }
}
