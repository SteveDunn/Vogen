using FluentAssertions;
using Vogen.Types;
using Xunit;

namespace Vogen.Tests;

public class FilenameTests
{
    [Fact]
    public void Replaces_ampersands_with_with_underscores() => new Filename("@").Value.Should().Be("_");

    [Fact]
    public void Stores_original_name()
    {
        var sut = new Filename("@record");
        sut.OriginalFilename.Should().Be("@record");
    }
    
    [Fact]
    public void Implicits_casts_to_string()
    {
        string s = new Filename("MyType");
        s.Should().Be("MyType");
    }
}