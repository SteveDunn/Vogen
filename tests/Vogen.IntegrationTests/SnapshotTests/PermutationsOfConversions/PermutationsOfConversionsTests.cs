using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Vogen.IntegrationTests.SnapshotTests.PermutationsOfConversions;

public class PermutationsOfConversionsTests
{
    /// <summary>
    /// For different types (class, struct, readonly struct), generate types with different
    /// permutations of conversions to ensure that everything builds OK.
    /// </summary>
    public class ConversionPermutationTests
    {
        // These used to be 'ClassData' tests, but they were run sequentially, which was very slow.
        // This test now runs the permutations in parallel.
        [Fact]
        public Task CompilesWithAnyCombinationOfConverters()
        {
            List<string> perms = new Permutations().ToList();

            string[] types = {"partial class", "partial struct", "partial readonly struct"};

            Parallel.ForEach(types, type =>
            {
                Parallel.ForEach(perms, conversions =>
                {
                    var settings = new VerifySettings();

                    // shorten the filename used
                    settings.UseParameters(type.Replace(' ', '-') + Hash(conversions));

                    RunTest($@"
  [ValueObject(conversions: {conversions}, underlyingType: typeof(int))]
  public {type} MyIntVo {{ }}", settings);
                });
            });

            return Task.CompletedTask;
        }
    }
    
    private static object Hash(string input)
    {
        using var sha1 = SHA1.Create();
        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));

        //make sure the hash is only alpha numeric to prevent characters that may break the url
        return string.Concat(Convert.ToBase64String(hash).ToCharArray().Where(char.IsLetterOrDigit).Take(10));
    }

    private static Task RunTest(string declaration, VerifySettings? settings = null)
    {
        var source = $@"using System;
using Vogen;
namespace Whatever
{{
{declaration}
}}";

        var (diagnostics, output) = TestHelper.GetGeneratedOutput<ValueObjectGenerator>(source);

        diagnostics.Should().BeEmpty();

        return Verifier.Verify(output, settings).UseDirectory("Snapshots");
    }    
}