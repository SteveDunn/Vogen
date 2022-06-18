using System;
using System.Collections.Generic;
using System.IO;

namespace Vogen.Generators.Conversions;

internal static class CodeSections
{
    public static string CutSection(string input, string identifier)
    {
        StringReader reader = new StringReader(input);
        StringWriter writer = new StringWriter();

        while (reader.ReadLine() is { } line)
        {
            if (!line.StartsWith(identifier))
            {
                writer.WriteLine(line);
            }
        }

        writer.Flush();

        var ret = writer.ToString();
        return !input.EndsWith(Environment.NewLine) ? ret.TrimEnd('\r', '\n') : ret;
    }

    public static string KeepSection(string input, string identifier)
    {
        StringReader reader = new StringReader(input);
        StringWriter writer = new StringWriter();

        while (reader.ReadLine() is { } line)
        {
            if (line.StartsWith(identifier))
            {
                line = line.Substring(identifier.Length);
            }

            writer.WriteLine(line);
        }

        writer.Flush();
        var ret = writer.ToString();
        return !input.EndsWith(Environment.NewLine) ? ret.TrimEnd('\r', '\n') : ret;
    }

    public static string CutSection2(string input, string identifier)
    {
        var indexes = AllIndexesOf(input, identifier);
        if (indexes.Count % 2 != 0)
        {
            throw new InvalidOperationException(
                $"Cannot cut section of '{identifier}' as there were {indexes.Count} instance(s), but was expecting even pairs.");
        }

        int i = indexes.Count -1;
        
        while (i > 0)
        {
            int end = indexes[i] + identifier.Length;
            int start = indexes[i-1];

            input = input.Remove(start, end - start);

            i -= 2;
        }

        return input;
    }

    public static List<int> AllIndexesOf(string str, string value)
    {
        List<int> indexes = new List<int>();
        for (int index = 0;; index += value.Length)
        {
            index = str.IndexOf(value, index, StringComparison.Ordinal);
            if (index == -1)
            {
                return indexes;
            }

            indexes.Add(index);
        }
    }
}