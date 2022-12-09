// ReSharper disable RedundantNameQualifier
// ReSharper disable ArrangeConstructorOrDestructorBody
// ReSharper disable PartialTypeWithSinglePart

namespace Vogen.Examples.SyntaxExamples
{
    /*
     * Value Objects can be in nested namespaces, but cannot be in a nested class.
     * This example below is OK as it's just a nested namespace.
     */
    namespace Namespace1
    {
        namespace Namespace2
        {
            [ValueObject]
            public partial struct NestedType
            {
            }
        }

        /*
         * This example below is not OK as it's a nested class.
         */
        internal class TopLevelClass
        {
            internal class AnotherClass
            {
                internal class AndAnother
                {
                    // uncomment to get error VOG001: Type 'NestedType' cannot be nested - remove it from inside AndAnother
                    // [ValueObject(typeof(int))]
                    public partial struct NestedType
                    {
                    }
                }
            }
        }
    }
}

