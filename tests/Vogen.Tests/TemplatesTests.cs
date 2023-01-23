using System;
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Vogen.Tests;

public class TemplatesTests
{

    private class Types : IEnumerable<object[]>
    {
        private readonly Type[] _types = new[]
        {
            typeof(bool), typeof(byte), typeof(char), typeof(DateOnly), typeof(DateTime), typeof(DateTimeOffset), typeof(decimal),
            typeof(double), typeof(float), typeof(Guid), typeof(System.Int32), typeof(long), typeof(short), typeof(string),
            typeof(TimeOnly)
        };

        private readonly string[] _technologies = new[]
        {
            "DapperTypeHandler", "EfCoreValueConverter", "LinqToDbValueConverter", "NewtonsoftJsonConverter", "SystemTextJsonConverter",
            "TypeConverter"
        };
        
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var eachType in _types)
            {
                foreach (var eachTech in _technologies)
                {
                    yield return new object[] { eachType, eachTech };
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
    [Theory]
    [ClassData(typeof(Types))]
    public void CanGetItemFromTypeName(Type type, string tech) => Templates.TryGetForSpecificType(type, tech).Should().NotBeNull();

    [Theory]
    [ClassData(typeof(Types))]
#pragma warning disable xUnit1026
    public void CanGetItemForAnyType(Type unused, string tech) => Templates.GetForAnyType(tech).Should().NotBeNull();
#pragma warning restore xUnit1026
}