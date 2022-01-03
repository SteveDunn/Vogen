using FluentAssertions;

using Xunit;

namespace Vogen.Tests.ToString
{
    public class ToStringTests
    {
        
        [Fact]
        public void To_string()
        {
            Age.From(18).ToString().Should().Be("18");
            Age.From(100).ToString().Should().Be("100");
            Age.From(1_000).ToString().Should().Be("1000");

            Name.From("fred").ToString().Should().Be("fred");
            Name.From("barney").ToString().Should().Be("barney");
            Name.From("wilma").ToString().Should().Be("wilma");
        }
    }

    [ValueObject(typeof(int))]
    public partial struct Age { }

    [ValueObject(typeof(string))]
    public partial struct Name { }
}