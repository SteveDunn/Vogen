using System;
using FluentAssertions;
using Vogen;
using Xunit;

namespace ConsumerTests.RecordTests;

public class RecordStructTests
{
    [Fact]
    public void GeneralBehaviour()
    {
        Types.MyRecordStruct r1 = Types.MyRecordStruct.From(123);
        r1.Value.Should().Be(123);

        Types.MyRecordStruct r2 = Types.MyRecordStruct.From(123);

        r1.Should().Be(r2);
        object.ReferenceEquals(r1, r2).Should().BeFalse();

        r1.Value.Should().Be(r2.Value);
        r1.GetHashCode().Should().Be(r2.GetHashCode());

        Types.MyRecordStruct r3 = r2 with
        {
            Value = 123
        };

        r3.Should().Be(r2);
        object.ReferenceEquals(r3, r2).Should().BeFalse();

        r3.Value.Should().Be(r2.Value);
        r3.GetHashCode().Should().Be(r2.GetHashCode());

        var msr1 = Types.MyStringRecordStruct.From(" normalizes     input ");
        msr1.Value.Should().Be("normalizesinput");
    }

    [Fact]
    public void WithBehaviour()
    {
        Types.MyRecordStruct r1 = Types.MyRecordStruct.From(123);
        Types.MyRecordStruct r2 = r1 with
        {
            Value = 123
        };

        r2.Should().Be(r1);

        Types.MyRecordStruct r3 = r2 with
        {
            Value = 0
        };

        r3.Should().NotBe(r2);
        r2.Should().Be(r1);

        r3.GetHashCode().Should().NotBe(r2.GetHashCode());
        r3.Value.Should().NotBe(r2.Value);

        Action a = () =>
        {
            Types.MyRecordStruct _ = r1 with { Value = -1 };
        };


        a.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("must be zero or more");
    }
}