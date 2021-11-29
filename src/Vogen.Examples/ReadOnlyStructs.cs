// ReSharper disable once RedundantUsingDirective
// ReSharper disable RedundantNameQualifier
// ReSharper disable ArrangeConstructorOrDestructorBody
// ReSharper disable StructCanBeMadeReadOnly

using Vogen.SharedTypes;

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
