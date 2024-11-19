using FluentAssertions;
using Vogen.Types;
using Xunit;

namespace Vogen.Tests;

public class SanitizedAssemblyNameTests
{
    [Fact]
    public void Replaces_dots_with_underscores() => new SanitizedAssemblyName(".").Value.Should().Be("_");
    
    [Fact]
    public void Strips_dll_from_end() => new SanitizedAssemblyName("foo.dll").Value.Should().Be("foo");

    [Fact]
    public void Strips_exe_from_end() => new SanitizedAssemblyName("foo.exe").Value.Should().Be("foo");

    [Fact]
    public void Treats_null_as_empty() => new SanitizedAssemblyName(null).Value.Should().Be(string.Empty);
}