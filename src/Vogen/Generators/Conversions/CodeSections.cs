using System;
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
}