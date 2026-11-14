using System;
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Vogen.Generators.Conversions;
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
            typeof(double), typeof(float), typeof(Guid), typeof(int), typeof(long), typeof(short), typeof(string),
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

    [Theory]
    [InlineData(typeof(uint), "System.UInt32")]
    [InlineData(typeof(ulong), "System.UInt64")]
    [InlineData(typeof(ushort), "System.UInt16")]
    [InlineData(typeof(sbyte), "System.SByte")]
    [InlineData(typeof(TimeSpan), "System.TimeSpan")]
    public void EfCoreValueConverter_uses_native_type_for_additional_primitives(Type underlyingClrType, string expectedUnderlyingType)
    {
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [CSharpSyntaxTree.ParseText("public readonly partial struct RowVersion;")],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]);

        var underlyingSymbol = compilation.GetTypeByMetadataName(underlyingClrType.FullName!)!;
        var voSymbol = compilation.GetTypeByMetadataName("RowVersion")!;

        Templates.IsKnownPrimitive(underlyingSymbol).Should().BeTrue();

        var generated = GenerateEfCoreTypes.GenerateInner(underlyingSymbol, isWrapperAValueType: true, voSymbol);
        generated.Should().Contain($"ValueConverter<RowVersion, {expectedUnderlyingType}>");
        generated.Should().NotContain("JsonSerializer");
    }
}