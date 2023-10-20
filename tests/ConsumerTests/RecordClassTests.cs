namespace ConsumerTests.RecordTests;

public class RecordClassTests
{
    [Fact]
    public void GeneralBehaviour()
    {
        Types.MyRecord r1 = Types.MyRecord.From(123);
        r1.Value.Should().Be(123);

        Types.MyRecord r2 = Types.MyRecord.From(123);

        r1.Should().Be(r2);
        object.ReferenceEquals(r1, r2).Should().BeFalse();

        r1.Value.Should().Be(r2.Value);
        r1.GetHashCode().Should().Be(r2.GetHashCode());

        Types.MyRecord r3 = r2 with
        {
            Value = 123
        };

        r3.Should().Be(r2);
        object.ReferenceEquals(r3, r2).Should().BeFalse();

        r3.Value.Should().Be(r2.Value);
        r3.GetHashCode().Should().Be(r2.GetHashCode());

        var msr1 = Types.MyStringRecord.From(" normalizes     input ");
        msr1.Value.Should().Be("normalizesinput");
    }

    [Fact]
    public void WithBehaviour()
    {
        Types.MyRecord r1 = Types.MyRecord.From(123);
        Types.MyRecord r2 = r1 with
        {
            Value = 123
        };

        r2.Should().Be(r1);

        Types.MyRecord r3 = r2 with
        {
            Value = 0
        };

        r3.Should().NotBe(r2);
        r2.Should().Be(r1);

        r3.GetHashCode().Should().NotBe(r2.GetHashCode());
        r3.Value.Should().NotBe(r2.Value);

        Action a = () =>
        {
            Types.MyRecord _ = r1 with {Value = -1};
        };


        a.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("must be zero or more");
    }

    [Fact]
    public void Using_init_still_throws()
    {
#pragma warning disable VOG010
        Types.MyRecord r1 = new Types.MyRecord { Value = 321 };
#pragma warning restore VOG010

        Action a = () => _ = r1.Value;

        a.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("Use of uninitialized Value Object*");
    }
}