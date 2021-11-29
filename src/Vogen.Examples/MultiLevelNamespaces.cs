// ReSharper disable once RedundantUsingDirective
// ReSharper disable RedundantNameQualifier
// ReSharper disable ArrangeConstructorOrDestructorBody
// // ReSharper disable StructCanBeMadeReadOnly

using Vogen.Examples.Namespace1.Namespace2;
using Vogen.SharedTypes;

namespace Vogen.Examples
{
    namespace Namespace1
    {
        namespace Namespace2
        {
            // uncomment to get error VOG001: Type 'NestedType' cannot be nested - remove it from inside AndAnother
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
