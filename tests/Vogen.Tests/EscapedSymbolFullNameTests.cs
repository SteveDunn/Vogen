using FluentAssertions;
using Microsoft.CodeAnalysis;
using NSubstitute;
using Vogen.Types;
using Xunit;

namespace Vogen.Tests;

public class EscapedSymbolFullNameTests
{
    private static INamespaceSymbol NamespaceSymbol(string dottedNamespace)
    {
        if (string.IsNullOrEmpty(dottedNamespace))
        {
            return null;
        }

        var lastDotPosition = dottedNamespace.LastIndexOf('.');
        
        var ns = Substitute.For<INamespaceSymbol>();
        
        (string Name, string Remainder) parts = lastDotPosition == -1 
            ? (dottedNamespace, null) 
            : (dottedNamespace[(lastDotPosition+1)..], dottedNamespace[..lastDotPosition]);
        
        ns.Name.Returns(parts.Name);
        var subNamespace = NamespaceSymbol(parts.Remainder);
        ns.ContainingNamespace.Returns(subNamespace);
            
        return ns;
    }
    
    [Fact]
    public void Includes_full_namespace()
    {
        var symbol = Substitute.For<INamedTypeSymbol>();
        
        symbol.Name.Returns("MyType");
        var namespaceSymbol = NamespaceSymbol("MyProject.Domain.Core");
        symbol.ContainingNamespace.Returns(namespaceSymbol);
        
        new EscapedSymbolFullName(symbol).Value.Should().Be("MyProject.Domain.Core.MyType");
    }

    [Fact]
    public void Escapes_type_name_with_ampersand_if_keyword()
    {
        var symbol = Substitute.For<INamedTypeSymbol>();
        
        symbol.Name.Returns("int");
        var namespaceSymbol = NamespaceSymbol("MyNamespace");
        symbol.ContainingNamespace.Returns(namespaceSymbol);
        
        new EscapedSymbolFullName(symbol).Value.Should().Be("MyNamespace.@int");
    }

    [Fact]
    public void Implicits_casts_to_string()
    {
        var symbol = Substitute.For<INamedTypeSymbol>();
        symbol.Name.Returns("MyType");

        var ns = NamespaceSymbol("MyNamespace");
        symbol.ContainingNamespace.Returns(ns);

        EscapedSymbolFullName sut = new(symbol);

        string s = sut;
        s.Should().Be("MyNamespace.MyType");
    }

    // [Theory]
    // [ClassData(typeof(KnownSymbolNamesData))]
    // public void Escapes_all_known_keywords(string keyword)
    // {
    //     var symbol = Substitute.For<INamedTypeSymbol>();
    //     symbol.FullName.Returns(keyword);
    //     var sut = new EscapedSymbolFullName(symbol);
    //     sut.Value.Should().Be("@" + keyword);
    //     sut.OriginalName.Should().Be(keyword);
    // }
}