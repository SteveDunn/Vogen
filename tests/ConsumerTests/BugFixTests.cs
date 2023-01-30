using System;
using @double;
using @bool.@byte.@short.@float.@object;
using FluentAssertions;
using Newtonsoft.Json;
using Vogen;
using Vogen.Tests.Types;
using Xunit;

namespace ConsumerTests.BugFixTests
{
    public class BugFixTests
    {
        /// <summary>
        /// Fixes bug https://github.com/SteveDunn/Vogen/issues/344 where a field that is a ValueObject and is null when
        /// deserialized by Newtonsoft.Json, throws an exception instead of returning null.
        /// </summary>
        [Fact]
        public void Bug344_Can_deserialze_a_null_field()
        {
            var p = new Person
            {
                Age = Age.From(42)
            };

            var serialized = JsonConvert.SerializeObject(p);
            var deserialized = JsonConvert.DeserializeObject<Person>(serialized)!;

            deserialized.Age.Should().Be(Age.From(42));
            deserialized.Name.Should().BeNull();
            deserialized.Address.Should().BeNull();
        }
    }

    public class Person
    {
        public Age Age { get; init; }
        
        public Name? Name { get; set; }
        
        public Address? Address { get; set; }
    }
    
    [ValueObject(conversions: Conversions.NewtonsoftJson)] public partial struct Age { }
    
    [ValueObject(typeof(string), conversions: Conversions.NewtonsoftJson)] public partial class Name { }
    
    [ValueObject(typeof(string), conversions: Conversions.NewtonsoftJson)] public partial struct Address { }
}