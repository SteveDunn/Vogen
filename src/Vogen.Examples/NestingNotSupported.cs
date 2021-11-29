// ReSharper disable once RedundantUsingDirective
using Vogen.SharedTypes;

namespace Vogen.Examples
{
    namespace AnotherNamespace
    {
        namespace AndAnother
        {
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
}
