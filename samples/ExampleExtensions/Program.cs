using Vogen;

[assembly: VogenDefaults(
    staticAbstractsGeneration: StaticAbstractsGeneration.MostCommon)]

Console.WriteLine("Hello, World!");

int supplied = 123;
int? notSupplied = null;

var non_null1 = MyId.From(supplied);
var non_null2 = MyId.FromNullable(supplied);
MyId? null_1 = MyId.FromNullable(notSupplied);

Write(non_null1);
Write(non_null2);
Write(null_1);
return;

static void Write(MyId? vo) => Console.WriteLine(vo?.Value.ToString() ?? "null");


[ValueObject<int>]
public partial class MyId;

static class Extensions
{
    extension<TWrapper, TPrimitive>(IVogen<TWrapper, TPrimitive>) where TWrapper : IVogen<TWrapper, TPrimitive> where TPrimitive : struct
    {
        public static TWrapper? FromNullable(TPrimitive? value) => value is null ? default : TWrapper.From(value.Value);
        
    }
}