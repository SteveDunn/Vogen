using FluentAssertions;
using Vogen.Tests.Types;
using Xunit;

namespace Vogen.Tests.InstanceFields
{
    public class InstanceFieldTests
    {
        [Fact]
        public void Type_with_two_instance_fields()
        {
            _ = MyIntWithTwoInstanceOfInvalidAndUnspecified.From(100);
        
            MyIntWithTwoInstanceOfInvalidAndUnspecified.Invalid.Value.Should().Be(-1);
            MyIntWithTwoInstanceOfInvalidAndUnspecified.Unspecified.Value.Should().Be(-2);
        }

        [Fact]
        public void Struct_with_an_instance_field()
        {
            var sut = MyIntStructWithADefaultOf22.From(100);
        
            sut.Value.Should().Be(100);
        
            MyIntStructWithADefaultOf22.Default.Value.Should().Be(22);
            MyIntStructWithADefaultOf22.Default2.Value.Should().Be(33);
        }
    }
}