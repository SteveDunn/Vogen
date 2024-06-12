using System.Globalization;
using FluentAssertions.Execution;

// The notion of 'Instances' is now obsolete. Instead, just use `new` (or `From`)
// in the constructor of your value objects.

namespace ConsumerTests.Instances;

[ValueObject(typeof(DateTime))]
[Instance(name: "iso8601_1", value: "2022-12-13")]
[Instance(name: "iso8601_2", value: "2022-12-13T13:14:15Z")]
[Instance(name: "ticks_as_long", value: 638064864000000000L)]
[Instance(name: "ticks_as_int", value: 2147483647)]
public readonly partial struct DateTimeInstance
{
}

[ValueObject(typeof(DateTimeOffset))]
[Instance(name: "iso8601_1", value: "2022-12-13")]
[Instance(name: "iso8601_2", value: "2022-12-13T13:14:15Z")]
[Instance(name: "ticks_as_long", value: 638064864000000000L)]
[Instance(name: "ticks_as_int", value: 2147483647)]
public readonly partial struct DateTimeOffsetInstance
{
}

[ValueObject(typeof(float))]
[Instance(name: "i1", value: 1.23f)]
[Instance(name: "i2", value: 2.34)]
[Instance(name: "i3", value: "3.45")]
[Instance(name: "i4", value: '2')]
public readonly partial struct MyFloatInstance
{
}

[ValueObject(typeof(decimal))]
[Instance(name: "i1", value: 1.23f)]
[Instance(name: "i2", value: 2.34)]
[Instance(name: "i3", value: "3.45")]
[Instance(name: "i4", value: '2')]
public readonly partial struct MyDecimalInstance
{
}

[ValueObject(typeof(double))]
[Instance(name: "i1", value: 1.23d)]
[Instance(name: "i2", value: 2.34)]
[Instance(name: "i3", value: "3.45")]
[Instance(name: "i4", value: '2')]
public readonly partial struct MyDoubleInstance
{
}

[ValueObject(typeof(char))]
[Instance(name: "i1", value: 1)]
[Instance(name: "i2", value: "2")]
[Instance(name: "i3", value: '3')]
public readonly partial struct MyCharInstance
{
}

[ValueObject(typeof(byte))]
[Instance(name: "i1", value: 1)]
[Instance(name: "i2", value: "2")]
[Instance(name: "i3", value: '3')]
public readonly partial struct MyByteInstance
{
}

public class InstanceTests
{
    public class DateTimeTests
    {
        [Fact]
        public void DateTimes()
        {
            using var _ = new AssertionScope();
            DateTimeInstance.iso8601_1.Value.Should().Be(new DateTime(2022, 12, 13));
            DateTimeInstance.iso8601_2.Value.Should().Be(new DateTime(2022, 12, 13, 13, 14, 15));
            DateTimeInstance.ticks_as_long.Value.Should().Be(new DateTime(2022, 12, 13));
            // ticks as an Int.MaxValue is 2147483647, which is 2,147,483,647 / 10m, which is ~214 seconds, which 3 minutes, 34 seconds
            DateTimeInstance.ticks_as_int.Value.Should().BeCloseTo(new DateTime(1, 1, 1, 0, 3, 34, 0), TimeSpan.FromTicks(7483647));
            //var l = new DateTimeOffset()
        }

        [Fact]
        [UseCulture("en-GB")]
        public void DateTimeOffsets()
        {
            using var _ = new AssertionScope();

            DateTimeOffsetInstance.iso8601_1.Value.UtcDateTime.Should()
                .Be(DateTime.Parse("2022-12-13", null, DateTimeStyles.RoundtripKind).ToUniversalTime());
            DateTimeOffsetInstance.iso8601_2.Value.Should().Be(new DateTimeOffset(2022, 12, 13, 13, 14, 15, TimeSpan.Zero));
            DateTimeOffsetInstance.ticks_as_long.Value.Should().Be(new DateTimeOffset(2022, 12, 13, 0, 0, 0, TimeSpan.Zero));
            DateTimeOffsetInstance.ticks_as_int.Value.Should().Be(new DateTimeOffset(2147483647, TimeSpan.Zero));
        }
    }

    [Fact]
    public void Basics()
    {
        using var _ = new AssertionScope();

        MyFloatInstance.i1.Value.Should().BeApproximately(1.23f, 0.01f);
        MyFloatInstance.i2.Value.Should().BeApproximately(2.34f, 0.01f);
        MyFloatInstance.i3.Value.Should().BeApproximately(3.45f, 0.01f);
        MyFloatInstance.i4.Value.Should().BeApproximately(2f, 0.01f);

        MyDecimalInstance.i1.Value.Should().Be(1.23m);
        MyDecimalInstance.i2.Value.Should().Be(2.34m);
        MyDecimalInstance.i3.Value.Should().Be(3.45m);
        MyDecimalInstance.i4.Value.Should().Be(2m);

        MyDoubleInstance.i1.Value.Should().BeApproximately(1.23f, 0.01f);
        MyDoubleInstance.i2.Value.Should().BeApproximately(2.34f, 0.01f);
        MyDoubleInstance.i3.Value.Should().BeApproximately(3.45f, 0.01f);
        MyDoubleInstance.i4.Value.Should().BeApproximately(2f, 0.01f);

        MyCharInstance.i1.Value.Should().Be('');
        MyCharInstance.i2.Value.Should().Be('2');
        MyCharInstance.i3.Value.Should().Be('3');

        MyByteInstance.i1.Value.Should().Be(1);
        MyByteInstance.i2.Value.Should().Be(2);
        MyByteInstance.i3.Value.Should().Be((byte) '3');
    }
}