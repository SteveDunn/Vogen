using System.Linq;
using System.Threading.Tasks;
using Vogen;

namespace SnapshotTests.ConversionPermutations;

public class PermutationsOfConversionsTests
{
    /// <summary>
    /// For different types (class, struct, readonly struct), generate types with different
    /// permutations of conversions to ensure that everything builds OK.
    /// </summary>
        public class ConversionPermutationTests
    {
        static readonly string[] _permutations = new Permutations().Distinct().ToArray();

        // These used to be 'ClassData' tests, but they were run sequentially, which was very slow.
        // This test now runs the permutations in parallel.
        [Fact]
        public async Task CompilesWithAnyCombinationOfConverters()
        {
            var tasks = Factory.TypeVariations.Select(t => Task.Run(() => Run(t))).ToArray();
            
            await Task.WhenAll(tasks);
        }

        private static async Task Run(string type)
        {
            foreach(var conversions in _permutations)
            {
                await RunTest($@"
  [ValueObject(conversions: {conversions}, underlyingType: typeof(int))]
  public {type} MyIntVo {{ }}", type, conversions);
            }
        }
    }
    
    private static Task RunTest(string declaration, string type, string conversions)
    {
        var source = $@"using System;
using Vogen;
namespace Whatever
{{
{declaration}
}}";

        return new SnapshotRunner<ValueObjectGenerator>()
            .WithSource(source)
            .CustomizeSettings(
                s =>
                {
                    var typeHash = type.Replace(' ', '-');
                    string parameters = typeHash + TestHelper.ShortenForFilename(conversions);
                    s.UseFileName(parameters);

                })
            .RunOnAllFrameworks();
    }    
}