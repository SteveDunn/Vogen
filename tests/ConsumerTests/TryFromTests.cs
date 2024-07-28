using Vogen.Tests.Types;

namespace ConsumerTests;

public class TryFromTests
{
    public class Using_result_type
    {
        [Fact]
        public void TryFrom_with_an_invalid_value_returns_an_error_with_the_validation_result()
        {
            ValueObjectOrError<Age> result = Age.TryFrom(17);
            result.IsSuccess.Should().BeFalse();
            result.Error.ErrorMessage.Should().Contain("Must be 18 or over");
        }

        [Fact]
        public void Cannot_access_the_value_object_from_a_failed_response()
        {
            ValueObjectOrError<Age> result = Age.TryFrom(17);
            result.IsSuccess.Should().BeFalse();
            var a = () => result.ValueObject;
            a.Should().ThrowExactly<InvalidOperationException>().WithMessage("Cannot access the value object as it is not valid: Must be 18 or over");
        }

        [Fact]
        public void TryFrom_with_a_valid_value_returns_the_value()
        {
            ValueObjectOrError<Age> result = Age.TryFrom(18);
            result.IsSuccess.Should().BeTrue();
            result.ValueObject.Should().Be(Age.From(18));
        }
    }

    public class Using_out_parameter
    {
        [Fact]
        public void TryFrom_with_an_invalid_value_returns_false_and_null()
        {
            bool ok = Age.TryFrom(17, out var age);
            ok.Should().BeFalse();
            age.Should().BeNull();
        }

        [Fact]
        public void TryFrom_with_a_valid_value_returns_the_value()
        {
            bool ok = Age.TryFrom(18, out var age);
            ok.Should().BeTrue();
            age.Should().Be(Age.From(18));
        }
    }

}