using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.ConversionPermutations;

public class Permutations : IEnumerable<string>
{
    private static readonly string[] _inputs =
    {
        "Conversions.TypeConverter", "Conversions.DapperTypeHandler", "Conversions.EfCoreValueConverter",
        "Conversions.NewtonsoftJson", "Conversions.SystemTextJson", "Conversions.LinqToDbValueConverter",
    };

    public IEnumerator<string> GetEnumerator()
    {
        yield return "Conversions.None";

        List<List<string>> l = new();
        

        for (int i = 0; i < _inputs.Length; i++)
        {
            l.Add(_inputs.Skip(i).ToList());   
        }
        
        foreach (List<string> s in l.Skip(1))
        {
            yield return string.Join(" | ", s);
        }

        l.Reverse();

        foreach (List<string> s in l)
        {
            yield return string.Join(" | ", s);
        }
        
        yield return string.Join(" | ", _inputs);

        yield return string.Join(" | ", _inputs.Reverse());
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}