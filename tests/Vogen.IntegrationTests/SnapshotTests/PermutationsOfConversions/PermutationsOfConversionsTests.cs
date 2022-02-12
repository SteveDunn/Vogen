using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Vogen.IntegrationTests.SnapshotTests.PermutationsOfConversions;

[UsesVerify] 
public class PermutationsOfConversionsTests
{
    [Theory]
    [ClassData(typeof(PermutationsOfConversions))]
    public Task PermutationsTests(string type, string conversions)
    {
        var settings = new VerifySettings();
        
        // shorten the filename used
        settings.UseParameters(type.Replace(' ', '-'), Hash(conversions));

         return RunTest($@"
  [ValueObject(conversions: {conversions}, underlyingType: typeof(int))]
  public {type} MyIntVo {{ }}", settings);
    }

    private object? Hash(string input)
    {
        using var sha1 = SHA1.Create();
        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));

        //make sure the hash is only alpha numeric to prevent characters that may break the url
        return string.Concat(Convert.ToBase64String(hash).ToCharArray().Where(x => char.IsLetterOrDigit(x)).Take(10));
    }

    private Task RunTest(string declaration, VerifySettings? settings = null)
    {
        var source = @"using System;
using Vogen;
namespace Whatever
{
" + declaration + @"
}";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();

        return Verifier.Verify(output, settings).UseDirectory("Snapshots");
    }
}