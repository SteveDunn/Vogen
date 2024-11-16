using System.Collections;
using System.Collections.Generic;

namespace Vogen.Tests;

public class KnownSymbolNamesData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return ["bool"];
        yield return ["byte"];
        yield return ["sbyte"];
        yield return ["short"];
        yield return ["ushort"];
        yield return ["int"];
        yield return ["uint"];
        yield return ["long"];
        yield return ["ulong"];
        yield return ["double"];
        yield return ["float"];
        yield return ["decimal"];
        yield return ["string"];
        yield return ["char"];
        yield return ["void"];
        yield return ["object"];
        yield return ["typeof"];
        yield return ["sizeof"];
        yield return ["true"];
        yield return ["false"];
        yield return ["if"];
        yield return ["else"];
        yield return ["while"];
        yield return ["for"];
        yield return ["foreach"];
        yield return ["do"];
        yield return ["switch"];
        yield return ["case"];
        yield return ["default"];
        yield return ["lock"];
        yield return ["try"];
        yield return ["throw"];
        yield return ["catch"];
        yield return ["finally"];
        yield return ["goto"];
        yield return ["break"];
        yield return ["continue"];
        yield return ["return"];
        yield return ["public"];
        yield return ["private"];
        yield return ["internal"];
        yield return ["protected"];
        yield return ["static"];
        yield return ["readonly"];
        yield return ["sealed"];
        yield return ["const"];
        yield return ["fixed"];
        yield return ["stackalloc"];
        yield return ["volatile"];
        yield return ["new"];
        yield return ["override"];
        yield return ["abstract"];
        yield return ["virtual"];
        yield return ["event"];
        yield return ["extern"];
        yield return ["ref"];
        yield return ["out"];
        yield return ["in"];
        yield return ["is"];
        yield return ["as"];
        yield return ["params"];
        yield return ["__arglist"];
        yield return ["__makeref"];
        yield return ["__reftype"];
        yield return ["__refvalue"];
        yield return ["this"];
        yield return ["base"];
        yield return ["namespace"];
        yield return ["using"];
        yield return ["class"];
        yield return ["struct"];
        yield return ["interface"];
        yield return ["enum"];
        yield return ["delegate"];
        yield return ["checked"];
        yield return ["unchecked"];
        yield return ["unsafe"];
        yield return ["operator"];
        yield return ["implicit"];
        yield return ["explicit"];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}