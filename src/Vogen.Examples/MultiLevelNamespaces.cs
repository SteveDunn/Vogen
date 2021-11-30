// ReSharper disable RedundantNameQualifier
// ReSharper disable ArrangeConstructorOrDestructorBody
// // ReSharper disable StructCanBeMadeReadOnly

using Vogen.SharedTypes;

namespace Vogen.Examples
{
    namespace Namespace1
    {
        namespace Namespace2
        {
            [ValueObject(typeof(int))]
            public partial struct NestedType
            {
            }
        }
    }

    public class UseIt
    {
        public UseIt()
        {
            Namespace1.Namespace2.NestedType.From(123);
        }
    }
}
