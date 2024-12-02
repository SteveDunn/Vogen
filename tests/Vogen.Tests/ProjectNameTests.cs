using FluentAssertions;
using Vogen.Types;
using Xunit;

namespace Vogen.Tests;

public class ProjectNameTests
{
    [Theory]
    [InlineData("MyAssembly.Core", "MyAssembly_Core")]
    [InlineData("MyAssembly . Core", "MyAssembly___Core")]
    [InlineData("CRM-API", "CRM_API")]
    [InlineData("My._Projéct_Oh.Yes_It.Is1plɹo_MollǝH", "My__Projéct_Oh_Yes_It_Is1plɹo_MollǝH")]
    [InlineData("", "")]
    public void Treats_spaces_hyphens_and_dots_with_with_underscores(string input, string output) =>
        ProjectName.FromAssemblyName(input).Value.Should().Be(output);

    [Fact]
    public void Implicitly_casts_to_string()
    {
        string s = ProjectName.FromAssemblyName("MyType");
        s.Should().Be("MyType");
    }
}