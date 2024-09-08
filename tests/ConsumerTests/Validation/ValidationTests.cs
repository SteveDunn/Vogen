using System.Diagnostics.CodeAnalysis;
using ConsumerTests;
using Vogen.Tests.Types;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace Vogen.Tests.ValidationTests;

[SuppressMessage("Usage", "VOG009:Using default of Value Objects is prohibited")]
public class ValidationTests
{
    [Fact]
    public void Validation_is_run()
    {
        {
            Action camelCase1 = () => MyVo_validate_with_camelCase_method_name.From(-1);
            camelCase1.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("must be greater than zero");

            bool b = MyVo_validate_with_camelCase_method_name.TryFrom(-1, out var vo);
            vo.Should().Be(default);
            b.Should().BeFalse();

            var voe = MyVo_validate_with_camelCase_method_name.TryFrom(-1);
            voe.IsSuccess.Should().BeFalse();
        }
        {
            Action pascalCase1 = () => MyVo_validate_with_PascalCase_method_name.From(-1);
            pascalCase1.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("must be greater than zero");
            
            bool b = MyVo_validate_with_PascalCase_method_name.TryFrom(-1, out var vo);
            vo.Should().Be(default);
            b.Should().BeFalse();

            var voe = MyVo_validate_with_PascalCase_method_name.TryFrom(-1);
            voe.IsSuccess.Should().BeFalse();

        }
    }

    [Fact]
    public void Validation_can_have_different_synonyms_for_input()
    {
        Action vo = () => MyVo_validate_with_synonymous_input_parameter.From(-1);

        vo.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("must be greater than zero");
    }

    [Fact]
    public void Validation_can_throw_a_different_exception()
    {
        Action vo = () => MyVo_throws_custom_exception.From(-1);

        vo.Should().ThrowExactly<InvalidAmountException>().WithMessage("must be greater than zero");
    }
    
    [SkippableIfBuiltWithNoValidationFlagFact]
    public void TryFrom_that_fails_for_a_struct_has_an_uninitialized_instance()
    {
        VoStruct.TryFrom(-1, out var vo).Should().BeFalse();
        vo.IsInitialized().Should().BeFalse();
    }

    [SkippableIfNotBuiltWithNoValidationFlagFact]
    public void TryFrom_that_fails_for_a_struct_has_an_default_instance()
    {
        VoStruct.TryFrom(-1, out var vo).Should().BeFalse();
        vo.IsInitialized().Should().BeTrue();
    }

    [Fact]
    public void Validation()
    {
        Func<Age> act = () => Age.From(12);
        act.Should().ThrowExactly<ValueObjectValidationException>();

        {
            var r = Age.TryFrom(12);
            r.IsSuccess.Should().BeFalse();
            r.Error.ErrorMessage.Should().Be("Must be 18 or over");
        }

        {
            bool b = Age.TryFrom(12, out var age);
            b.Should().BeFalse();
            age.Should().BeNull();
        }
        
        Func<EightiesDate> act2 = () => EightiesDate.From(new DateTime(1990, 1, 1));
        act2.Should().ThrowExactly<ValueObjectValidationException>();

        Func<EightiesDate> act3 = () => EightiesDate.From(new DateTime(1985, 6, 10));
        act3.Should().NotThrow();

        string[] validDaves = new[] { "dave grohl", "david beckham", "david bowie" };
        foreach (var name in validDaves)
        {
            Func<Dave> act4 = () => Dave.From(name);
            act4.Should().NotThrow();

            {
                var r = Dave.TryFrom(name);
                r.IsSuccess.Should().BeTrue();
            }

            {
                bool b = Dave.TryFrom(name, out var r);
                b.Should().BeTrue();
                r.Value.Should().Be(name);
            }
        }

        string[] invalidDaves = new[] { "dafid jones", "fred flintstone", "davidoff cool water" };
        foreach (var name in invalidDaves)
        {
            Func<Dave> act5 = () => Dave.From(name);
            act5.Should().ThrowExactly<ValueObjectValidationException>();
            
            {
                var r = Dave.TryFrom(name);
                r.IsSuccess.Should().BeFalse();
                r.Error.ErrorMessage.Should().Be("must be a dave or david");
            }

            {
                bool b = Dave.TryFrom(name, out var r);
                b.Should().BeFalse();
                r.Should().BeNull();
            }
        }

        Func<MinimalValidation> act6 = () => MinimalValidation.From(1);
        act6.Should().ThrowExactly<ValueObjectValidationException>().WithMessage("[none provided]");
    }

    [Fact]
    public void No_validation()
    {
        Func<Anything> act = () => Anything.From(int.MaxValue);
        act.Should().NotThrow();
        
        bool b = Anything.TryFrom(-1, out var vo);
        vo.Value.Should().Be(-1);
        b.Should().BeTrue();

        var voe = Anything.TryFrom(-1);
        voe.IsSuccess.Should().BeTrue();
        voe.ValueObject.Value.Should().Be(-1);
    }
}
