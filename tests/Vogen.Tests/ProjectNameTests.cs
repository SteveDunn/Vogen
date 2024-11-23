using FluentAssertions;
using Vogen.Types;
using Xunit;

namespace Vogen.Tests;

public class ProjectNameTests
{
    [Theory]
    [InlineData("MyAssembly.Core", "MyAssemblyCore")]
    [InlineData("MyAssembly . Core", "MyAssemblyCore")]
    [InlineData("My._Projéct_Oh.Yes_It.Is1plɹo_MollǝH", "My_Projéct_OhYes_ItIs1plɹo_MollǝH")]
    public void Removes_spaces_commas_and_dots_with_with_underscores(string input, string output) =>
        ProjectName.FromAssemblyName(input).Value.Should().Be(output);

    [Fact]
    public void Implicitly_casts_to_string()
    {
        string s = ProjectName.FromAssemblyName("MyType");
        s.Should().Be("MyType");
    }
}