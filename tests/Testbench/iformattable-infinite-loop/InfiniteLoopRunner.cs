using System.Reflection.Metadata.Ecma335;
using Vogen;

namespace iformattable_infinite_loop;
using System;

public class InfiniteLoopRunner
{
    public static void Run()
    {
        var testContainer = new TestContainer();

        Console.WriteLine(testContainer);
    }

    public class TestContainer
    {
        // uninitialized prints `[UNINITIALIZED]`
        public StructVoString StructVoString { get; set; }
        
        // prints nothing, null references aren't called
        public ClassMyType ClassMyType { get; set; } = default!;
        
        // prints nothing
        public StructVoMyTypeClassFormattable StructVoMyTypeClassFormattable { get; set; }
        
        // default guid (000-00000 etc)
        public Guid TheGuid { get; set; }
        
        // nothing - null references are not called
        public string TheString { get; set; } = default!;

        public override string ToString()
            => $"StructVoString:{StructVoString} - MyType:{ClassMyType} - TheGuid:{TheGuid} - TheString:{TheString} - StructVoMyTypeClassFormattable{StructVoMyTypeClassFormattable}";
    }

}

[ValueObject<string>]
public readonly partial struct StructVoString;

[ValueObject<ClassMyType>]
public readonly partial struct StructVoMyTypeClassFormattable;

public class ClassMyType : ISpanFormattable
{
    private string? _value;

    public ClassMyType(string? value) => _value = value;
    public string ToString(string? format, IFormatProvider? formatProvider) => _value ?? "[UNINITIALIZED]";

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        if (_value is not null)
        {
            if (destination.Length < _value.Length)
            {
                charsWritten = 0;
                return false;
            }


            _value.AsSpan().CopyTo(destination);
            charsWritten = _value.Length;
            return true;
        }

        charsWritten = 0;
        return true;
    }
}
