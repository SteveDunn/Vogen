using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Vogen.IntegrationTests.SnapshotTests.PermutationsOfConversions;

public class PermutationsOfConversions : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        string[] types = new[] {"partial class", "partial struct", "readonly partial struct"};

        string[] inputs =
        {
            "Conversions.TypeConverter", "Conversions.DapperTypeHandler", "Conversions.EfCoreValueConverter",
            "Conversions.NewtonsoftJson", "Conversions.SystemTextJson", "Conversions.LinqToDbValueConverter",
        };

        foreach (var eachType in types)
        {
            yield return new object[] {eachType, "Conversions.None"};

            for (int i = 0; i < inputs.Length - 2; i++)
            {
                var subset = inputs.Skip(i).ToArray();
                var permutations = Permute(subset);
                foreach (var perms in permutations)
                {
                    yield return new object[]
                    {
                        eachType,
                        string.Join(" | ", perms)
                    };
                }
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private static IEnumerable<IList<string>> Permute(string[] strings)
    {
        var list = new List<IList<string>>();
        return DoPermute(strings, 0, strings.Length - 1, list);
    }

    private static IList<IList<string>> DoPermute(string[] strings, int start, int end, IList<IList<string>> list)
    {
        if (start == end)
        {
            list.Add(new List<string>(strings));
        }
        else
        {
            for (var i = start; i <= end; i++)
            {
                Swap(ref strings[start], ref strings[i]);
                DoPermute(strings, start + 1, end, list);
                Swap(ref strings[start], ref strings[i]);
            }
        }

        return list;
    }

    private static void Swap(ref string a, ref string b) => (a, b) = (b, a);
}