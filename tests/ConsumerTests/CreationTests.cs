using System.Runtime.InteropServices;
using @double;
using @bool.@byte.@short.@float.@object;
using Vogen.Tests.Types;

namespace NotSystem
{
    public static class Activator
    {
        public static T? CreateInstance<T>() => default(T);
    }
}

namespace ConsumerTests
{
    public class CreationTests
    {
        [Fact]
        public void Creation_Happy_Path_MyIntGeneric()
        {
            MyIntGeneric vo1 = MyIntGeneric.From(123);
            int n = Marshal.SizeOf<MyIntStruct>();
            int n2 = Marshal.SizeOf<int>();
            MyIntGeneric vo2 = MyIntGeneric.From(123);

            vo1.Should().Be(vo2);
            (vo1 == vo2).Should().BeTrue();
        }

        // There is an analyzer that stops creation of VOs via Activator.CreateInstance.
        // This test is here to ensure that it *only* catches System.Activator.
        // If compilation fails here, then that logic is broken.
        [Fact]
        public void Allows_using_Activate_CreateInstance_from_another_namespace()
        {
            _ = NotSystem.Activator.CreateInstance<string>();
        }

        [Fact]
        public void Creation_Happy_Path_MyInt()
        {
            MyInt vo1 = MyInt.From(123);
            MyInt vo2 = MyInt.From(123);

            vo1.Should().Be(vo2);
            (vo1 == vo2).Should().BeTrue();
        }

        [Fact]
        public void Creation_Happy_Path_MyString()
        {
            MyString vo1 = MyString.From("123");
            MyString vo2 = MyString.From("123");

            vo1.IsInitialized().Should().BeTrue();
            vo2.IsInitialized().Should().BeTrue();

            vo1.Should().Be(vo2);
            (vo1 == vo2).Should().BeTrue();
        }

        [Fact]
        public void Creation_Unhappy_Path_MyString()
        {
            Action action = () => MyString.From(null!);

            action.Should().Throw<ValueObjectValidationException>().WithMessage("Cannot create a value object with null.");
        }

        [Fact]
        public void Creation_Unhappy_Path_MyInt()
        {
            Action action = () => MyInt.From(-1);

            action.Should().Throw<ValueObjectValidationException>().WithMessage("must be greater than zero");
        }

        [Fact]
        public void Default_vo_throws_at_runtime()
        {
            CustomerId[] ints = new CustomerId[10];
            Func<int> action = () => ints[0].Value;

            action.Should().Throw<ValueObjectValidationException>().WithMessage("Use of uninitialized Value Object*");
        }

        [Fact]
        public void Creation_can_create_a_VO_with_a_verbatim_identifier()
        {
            @class c1 = @class.From(123);
            @class c2 = @class.From(123);

            c1.Should().Be(c2);
            (c1 == c2).Should().BeTrue();

            @event e1 = @event.From(123);
            @event e2 = @event.From(123);

            e1.Should().Be(e2);
            (e1 == e2).Should().BeTrue();
        }

        [Fact]
        public void Creation_can_create_a_VO_with_a_verbatim_name_and_verbatim_underlying_from_a_verbatim_namespace()
        {
            @event2 e1 = @event2.From(new @record.@struct.@float.@decimal());
            @event2 e2 = @event2.From(new @record.@struct.@float.@decimal());

            e1.Should().Be(e2);
            (e1 == e2).Should().BeTrue();
        }

        [Fact]
        public void Creation_can_create_a_VO_with_from_a_namespace_with_an_escaped_keyword()
        {
            @classFromEscapedNamespace c1 = @classFromEscapedNamespace.From(123);
            @classFromEscapedNamespace c2 = @classFromEscapedNamespace.From(123);

            c1.Should().Be(c2);
            (c1 == c2).Should().BeTrue();
        }

        [Fact]
        public void Underlying_types_can_have_escaped_keywords()
        {
            @classFromEscapedNamespace c1 = @classFromEscapedNamespace.From(123);
            @classFromEscapedNamespace c2 = @classFromEscapedNamespace.From(123);

            c1.Should().Be(c2);
            (c1 == c2).Should().BeTrue();
        }
    }
}