using System;
using Vogen;

[assembly: VogenDefaults(
    typeof(string),
    Conversions.TypeConverter | Conversions.SystemTextJson,
    customizations: Customizations.TreatNumberAsStringInSystemTextJson,
    debuggerAttributes: DebuggerAttributeGeneration.Basic
)]


var vo = MyVo.From("aaa");
Console.WriteLine(vo.Value);

[ValueObject]
public partial class MyVo
{
    
}
