// using FluentAssertions;
// using Microsoft.CodeAnalysis;
// using NSubstitute;
// using Vogen.Types;
// using Xunit;
//
// namespace Vogen.Tests;
//
// public class EscapedSymbolFullNameTests
// {
//     [Fact]
//     public void Escapes_with_ampersand_if_keyword()
//     {
//         var symbol = Substitute.For<INamedTypeSymbol>();
//         symbol.Name.Returns("int");
//         
//         new EscapedSymbolFullName(symbol).Value.Should().Be("@int");
//     }
//
//     [Fact]
//     public void Implicits_casts_to_string()
//     {
//         var symbol = Substitute.For<INamedTypeSymbol>();
//         symbol.Name.Returns("int");
//
//         EscapedSymbolFullName sut = new(symbol);
//
//         string s = sut;
//         s.Should().Be("@int");
//     }
//
//     // [Theory]
//     // [ClassData(typeof(KnownSymbolNamesData))]
//     // public void Escapes_all_known_keywords(string keyword)
//     // {
//     //     var symbol = Substitute.For<INamedTypeSymbol>();
//     //     symbol.FullName.Returns(keyword);
//     //     var sut = new EscapedSymbolFullName(symbol);
//     //     sut.Value.Should().Be("@" + keyword);
//     //     sut.OriginalName.Should().Be(keyword);
//     // }
// }