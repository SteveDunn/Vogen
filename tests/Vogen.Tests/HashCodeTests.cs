using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Vogen.HashCodeTests
{
    [ValueObject(typeof(int))]
    public partial struct MyStructInt { }

    [ValueObject(typeof(int))]
    public partial struct MyStructInt2 { }
    
    
    public class HashCodeTests
    {
        [Fact]
        public void SameStructsHaveSameHashCode()
        {
            MyStructInt.From(0).GetHashCode().Should().Be(MyStructInt.From(0).GetHashCode());
            MyStructInt.From(0).GetHashCode().Should().Be(MyStructInt2.From(0).GetHashCode());
            
            MyStructInt.From(-1).GetHashCode().Should().Be(MyStructInt.From(-1).GetHashCode());
            MyStructInt.From(-1).GetHashCode().Should().Be(MyStructInt2.From(-1).GetHashCode());
        }

        [Fact]
        public void DifferentStructsHaveDifferentHashCode()
        {
            var hc1 = MyStructInt.From(-1).GetHashCode();
            var hc2 = MyStructInt.From(0).GetHashCode();
            
            hc1.Should().NotBe(hc2);
            MyStructInt.From(-1).GetHashCode().Should().NotBe(MyStructInt2.From(0).GetHashCode());
        }
    }
}
