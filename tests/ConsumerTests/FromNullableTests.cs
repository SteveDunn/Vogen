using Vogen.Tests.Types;

namespace ConsumerTests;

[ValueObject(typeof(Guid))]
public partial class GuidId;

public class FromNullableTests
{
    public class Test_From_Nullable
    {
        [Fact]
        public void Int()
        {
            Age? nullAge = Age.FromNullable(null);
            Assert.Null(nullAge);
            
            Age? notNullAge = Age.FromNullable(32);
            Assert.Equal(32, notNullAge?.Value);

            Assert.Throws<ValueObjectValidationException>(() =>
            {
                Age? invalidDage = Age.FromNullable(10);
            });
        }
        
        [Fact]
        public void String()
        {
            Dave? nullDave = Dave.FromNullable(null);
            Assert.Null(nullDave);
            
            Dave? notNullDave = Dave.FromNullable("Dave Angel");
            Assert.Equal("Dave Angel", notNullDave?.Value);

            Assert.Throws<ValueObjectValidationException>(() =>
            {
                Dave? notDave = Dave.FromNullable("Elvis Presley");
            });
        }
        
        [Fact]
        public void Guid()
        {
            GuidId? nullId = GuidId.FromNullable(null);
            Assert.Null(nullId);

            var id = System.Guid.NewGuid();
            GuidId? notNullId = GuidId.FromNullable(id);
            Assert.Equal(notNullId?.Value, id);
        }
    }
}