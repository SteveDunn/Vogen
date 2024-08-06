using System;
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Vogen.Tests;

public class TemplatesTests
{
    [Theory]
    [ClassData(typeof(SpecificTypes))]
    public void CanGetItemFromTypeName(Type type, string tech) => Templates.TryGetForSpecificType(type, tech).Should().NotBeNull();

    [Theory]
    [ClassData(typeof(AnyTypes))]
#pragma warning disable xUnit1026
    public void CanGetItemForAnyType(Type unused, string tech) => Templates.GetForAnyType(tech).Should().NotBeNull();
#pragma warning restore xUnit1026
    
    private class Types : IEnumerable<object[]>
    {
        private readonly string[] _technologies;

        protected Types(string[] technologies) => _technologies = technologies;

        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var eachType in _types)
            {
                foreach (var eachTech in _technologies)
                {
                    yield return [eachType, eachTech];
                }
            }
        }
        
        private static readonly Type[] _types =
        [
            typeof(bool), typeof(byte), typeof(char), typeof(DateOnly), typeof(DateTime), typeof(DateTimeOffset), typeof(decimal),
            typeof(double), typeof(float), typeof(Guid), typeof(System.Int32), typeof(long), typeof(short), typeof(string),
            typeof(TimeOnly)
        ];

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private class AnyTypes : Types
    {
        public AnyTypes() : base(_technologies)
        {
        }

        private static readonly string[] _technologies =
        [
            "DapperTypeHandler", "LinqToDbValueConverter", "NewtonsoftJsonConverterReferenceType", 
            "NewtonsoftJsonConverterValueType", "SystemTextJsonConverter", "TypeConverter"
        ];
    }

    private class SpecificTypes : Types
    {
        public SpecificTypes() : base(_technologies)
        {
        }

        private static readonly string[] _technologies =
        [
            "DapperTypeHandler", "LinqToDbValueConverter", "SystemTextJsonConverter", "TypeConverter"
        ];
    }
}