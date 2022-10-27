using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.ConversionPermutations;

public class Permutations : IEnumerable<string>
{
    static readonly string[] _inputs =
    {
        "Conversions.TypeConverter", "Conversions.DapperTypeHandler", "Conversions.EfCoreValueConverter",
        "Conversions.NewtonsoftJson", "Conversions.SystemTextJson", "Conversions.LinqToDbValueConverter",
    };

    public IEnumerator<string> GetEnumerator()
    {
        yield return "Conversions.None";

        for (int i = 0; i < _inputs.Length - 2; i++)
        {
            var subset = _inputs.Skip(i).ToArray();
            var permutations = Permute(subset);
            foreach (var perms in permutations)
            {
                yield return string.Join(" | ", perms);
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