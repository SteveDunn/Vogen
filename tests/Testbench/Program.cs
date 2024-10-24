using System;
using Vogen;
using Vogen.EfCoreTest;

[assembly: VogenDefaults(
    staticAbstractsGeneration: StaticAbstractsGeneration.MostCommon | StaticAbstractsGeneration.InstanceMethodsAndProperties, 
    conversions: Conversions.EfCoreValueConverter)]

namespace Testbench;

[ValueObject<DateTimeOffset>]
public partial struct DateOfBirth;

public class B
{
    /// <summary>
    /// Summary in the base class
    /// </summary>
    /// <returns>the string, obvs!</returns>
    public override string ToString() => base.ToString()!;
}

public class D : B
{
    //public override string ToString() => base.ToString()!;
}

class CC
{
    /// <summary>
    /// This is my text.
    ///  <para>
    /// <inheritdoc/>
    /// </para>
    /// </summary>
    /// <returns>
    /// blah blah
    /// <para>
    /// <inheritdoc cref="D" />
    /// </para>
    /// </returns>
    public override string ToString() => base.ToString()!;
}

public static class Program
{
    public static void Main()
    {

        new CC().ToString();
        var x = MyCustomVo.From(new C()).ToString();
        var dob = DateOfBirth.From(DateTimeOffset.Now);
        Console.WriteLine(dob.ToString(""));
        DoubleVo.From(1.23).ToString("a");
    }
}

// public class X
// {
//     public string ToString() => "!!";
// }

[ValueObject<DateOnly>(conversions: Conversions.None)]
public partial struct CreationDate
{
    public string ToString(string format) => "!!";
}

[ValueObject(typeof(double))]
public partial struct DoubleVo { }

public class C : IParsable<C>
{
    public static C Parse(string s, IFormatProvider? provider) => throw new NotImplementedException();

    public static bool TryParse(string? s, IFormatProvider? provider, out C result) => throw new NotImplementedException();
}

[ValueObject<C>]
public partial struct MyCustomVo
{
    /// <inheritdoc cref = "M:Testbench.C.ToString()"/>
    public readonly override global::System.String ToString() => IsInitialized() ? Value.ToString() ?? "" : "[UNINITIALIZED]";

}

