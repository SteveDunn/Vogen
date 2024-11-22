using System.Globalization;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace ConsumerTests.ParsingAndFormattingTests;

public class Tests
{
    [Fact]
    [UseCulture("en-GB")]
    public void Integers()
    {
        {
            StructIntVoNoValidation.Parse("1024").Should().Be(StructIntVoNoValidation.From(1024));
            StructIntVoNoValidation.Parse("1,024", NumberStyles.AllowThousands).Should().Be(StructIntVoNoValidation.From(1024));
            
            StructIntVoNoValidation.TryParse("1024", out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(1024);
        }
        {
            StructIntVoNoValidation.TryParse("1,000", NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(1000);
        }

        
        {
            ClassIntVoNoValidation.Parse("1024").Should().Be(ClassIntVoNoValidation.From(1024));
            ClassIntVoNoValidation.Parse("1,024", NumberStyles.AllowThousands).Should().Be(ClassIntVoNoValidation.From(1024));
            
            ClassIntVoNoValidation.TryParse("1024", out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(1024);
        }
        {
            ClassIntVoNoValidation.TryParse("1,000", NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(1000);
        }

        
        {
            RecordClassIntVoNoValidation.Parse("1024").Should().Be(RecordClassIntVoNoValidation.From(1024));
            RecordClassIntVoNoValidation.Parse("1,024", NumberStyles.AllowThousands).Should().Be(RecordClassIntVoNoValidation.From(1024));
            
            RecordClassIntVoNoValidation.TryParse("1024", out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(1024);
        }
        {
            RecordClassIntVoNoValidation.TryParse("1,000", NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(1000);
        }

        {
            RecordStructIntVoNoValidation.Parse("1024").Should().Be(RecordStructIntVoNoValidation.From(1024));
            RecordStructIntVoNoValidation.Parse("1,024", NumberStyles.AllowThousands).Should().Be(RecordStructIntVoNoValidation.From(1024));
            
            RecordStructIntVoNoValidation.TryParse("1024", out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(1024);
        }
        {
            RecordStructIntVoNoValidation.TryParse("1,000", NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(1000);
        }
    }

    [Fact]
    public void Decimals()
    {
        {
            DecimalVo.Parse("1024.56", NumberStyles.Any, CultureInfo.InvariantCulture).Should().Be(DecimalVo.From(1024.56m));
        }

        {
            DecimalVo.TryParse("1024.56", NumberStyles.Any, CultureInfo.InvariantCulture, out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(1024.56m);
        }

        {
            DecimalVo.TryParse("1,000.25", NumberStyles.Number, CultureInfo.InvariantCulture, out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(1000.25m);
        }

        {
            DecimalVo.TryParse("1.000,25", NumberStyles.Number, new CultureInfo("de"), out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(1000.25m);
        }
    }

    [Fact]
    public void Bytes()
    {
        ByteVo.Parse("12").Should().Be(ByteVo.From(12));
        ByteVo.TryParse("12", out var ivo).Should().BeTrue();
        ivo.Value.Should().Be(12);
    }

    [SkippableIfBuiltWithNoValidationFlagFact]
    public void Bools()
    {
        StructBoolVo.Parse("True").Should().Be(StructBoolVo.From(true));
        
        {
            StructBoolVo.TryParse("True", out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(true);
        }

        {
            StructBoolVo.TryParse("TrueDat", out var ivo).Should().BeFalse();
            Func<bool> a = () => ivo.Value;
            a.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("Use of uninitialized Value Object*");
        }
        
        ClassBoolVo.Parse("True").Should().Be(ClassBoolVo.From(true));
        
        {
            ClassBoolVo.TryParse("True", out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(true);
        }
        
        {
            ClassBoolVo.TryParse("TrueDat", out var ivo).Should().BeFalse();
            Func<bool> a = () => ivo.Value;
            a.Should().NotThrow<ValueObjectValidationException>();
            ivo.Should().Be(null);
        }

        RecordClassBoolVo.Parse("True").Should().Be(RecordClassBoolVo.From(true));
        
        {
            RecordClassBoolVo.TryParse("True", out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(true);
        }
        
        {
            RecordClassBoolVo.TryParse("TrueDat", out var ivo).Should().BeFalse();
            Func<bool> a = () => ivo.Value;
            a.Should().NotThrow<ValueObjectValidationException>();
            ivo.Should().Be(null);
        }
        
        RecordStructBoolVo.Parse("True").Should().Be(RecordStructBoolVo.From(true));
        
        {
            RecordStructBoolVo.TryParse("True", out var ivo).Should().BeTrue();
            ivo.Value.Should().Be(true);
        }
        {
            RecordStructBoolVo.TryParse("TrueDat", out var ivo).Should().BeFalse();
            Func<bool> a = () => ivo.Value;
            a.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("Use of uninitialized Value Object*");
        }
    }

    [Fact]
    public void Chars()
    {
        CharVo.Parse("a").Should().Be(CharVo.From('a'));
        CharVo.TryParse("a", out var ivo).Should().BeTrue();
        ivo.Value.Should().Be('a');
    }

    [Fact]
    public void Double()
    {
        DoubleVo.Parse("123.45", NumberStyles.Any, CultureInfo.InvariantCulture).Should().Be(DoubleVo.From(123.45d));
        DoubleVo.TryParse("123.45", NumberStyles.Any, CultureInfo.InvariantCulture, out var ivo).Should().BeTrue();
        ivo.Value.Should().Be(123.45);
    }

    [SkippableIfBuiltWithNoValidationFlagFact]
    public void When_parsing_fails_for_structs()
    {
        {
            Action a = () => StructIntVo.Parse("fifty");

            a.Should().ThrowExactly<FormatException>();
        }

        {
            StructIntVo.TryParse("fifty", out var ivo).Should().BeFalse();

            Action a = () => _ = ivo.Value;

            a.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("Use of uninitialized Value Object*");
        }
    }

    [SkippableIfBuiltWithNoValidationFlagFact]
    public void When_parsing_fails_for_record_structs()
    {
        {
            Action a = () => RecordStructIntVo.Parse("fifty");

            a.Should().ThrowExactly<FormatException>();
        }

        {
            RecordStructIntVo.TryParse("fifty", out var ivo).Should().BeFalse();

            Action a = () => _ = ivo.Value;

            a.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("Use of uninitialized Value Object*");
        }
    }

    [Fact]
    public void When_parsing_fails_for_classes()
    {
        {
            Action a = () => ClassIntVo.Parse("fifty");

            a.Should().ThrowExactly<FormatException>();
        }

        {
            ClassIntVo.TryParse("fifty", out var ivo).Should().BeFalse();

            ivo.Should().BeNull();
        }
    }

    [Fact]
    public void When_parsing_fails_for_record_classes()
    {
        {
            Action a = () => RecordClassIntVo.Parse("fifty");

            a.Should().ThrowExactly<FormatException>();
        }

        {
            RecordClassIntVo.TryParse("fifty", out var ivo).Should().BeFalse();

            ivo.Should().BeNull();
        }
    }

    [Fact]
    public void When_parsing_succeeds_but_fails_validation_an_exception_is_thrown()
    {
        Action a = () => StructIntVo.Parse("100");

        a.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("must be less than 100");
    }

    [SkippableIfBuiltWithNoValidationFlagFact]
    public void When_try_parse_succeeds_but_fails_validation_a_default_instance_exists_and_try_parse()
    {
        StructIntVo.TryParse("100", out var sut).Should().BeFalse();
        
        Func<int> f = () => sut.Value;
        f.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("Use of uninitialized Value Object*");
    }

    [SkippableIfBuiltWithNoValidationFlagFact]
    public void Try_parse_calls_the_provided_normalize_method()
    {
        {
            // the normalization halves the number, so it passes validation.
            StructIntVoWithNormalization.TryParse("120", out var sut).Should().BeTrue();
            sut.Value.Should().Be(60);
        }

        {
            // the normalization halves the number, but it's so big to start with that it fails validation.
            StructIntVoWithNormalization.TryParse("200", out var sut).Should().BeFalse();

            Func<int> f = () => sut.Value;
            f.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("Use of uninitialized Value Object*");
        }
    }

    [Fact]
    public void With_own_instance_parse_method_it_still_generates_static_method()
    {
        // doesn't really make much sense as an instance method, but
        // we still want to test it.
        var instance = VoWithOwnInstanceParseMethod.From("123");
        instance.Parse("123").Value.Should().Be("321");
    }

    [Fact]
    public void With_own_static_parse_method_it_still_generates_static_method()
    {
        VoStringWithOwnStaticParseMethod.Parse("123").Value.Should().Be("321");
    }

    [Fact]
    public void With_own_static_parse_method_and_format_provider_it_uses_the_one_provided()
    {
        VoStringWithOwnStaticParseMethodWithFormatProvider.Parse("123", CultureInfo.InvariantCulture).Value.Should().Be("321");
    }
}