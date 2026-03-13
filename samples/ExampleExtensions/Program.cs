using Vogen;

// Generates the IVogen<TWrapper, TPrimitive> interface, which can then be used in C# 14 extension members.
[assembly: VogenDefaults(
    staticAbstractsGeneration: StaticAbstractsGeneration.MostCommon)]

int suppliedId = 123;
int? nullId = null;

var non_null_id_1 = CustomerId.From(suppliedId);
var non_null_id_2 = CustomerId.FromNullable(suppliedId);
CustomerId? null_id_1 = CustomerId.FromNullable(nullId);

string suppliedName = "Frescho";
string? nullName = null;

var non_null_name_1 = CustomerName.From(suppliedName);
var non_null_name_2 = CustomerName.FromNullable(suppliedName);
CustomerName? null_name_1 = CustomerName.FromNullable(nullName);

WriteId(non_null_id_1);
WriteId(non_null_id_2);
WriteId(null_id_1);

WriteName(non_null_name_1);
WriteName(non_null_name_2);
WriteName(null_name_1);
return;

static void WriteId(CustomerId? vo) => Console.WriteLine(vo?.Value.ToString() ?? "null");
static void WriteName(CustomerName? vo) => Console.WriteLine(vo?.Value ?? "null");


[ValueObject<int>]
public partial class CustomerId;

[ValueObject<string>]
public partial class CustomerName;

internal static class Extensions
{
    extension<TWrapper, TPrimitive>(IVogen<TWrapper, TPrimitive>) 
        where TWrapper : IVogen<TWrapper, TPrimitive> 
        where TPrimitive : struct
    {
        public static TWrapper? FromNullable(TPrimitive? value) => value is null ? default : TWrapper.From(value.Value);
    }
    
    extension<TWrapper, TPrimitive>(IVogen<TWrapper, TPrimitive>)
        where TWrapper : IVogen<TWrapper, TPrimitive>
        where TPrimitive : class
    {
        public static TWrapper? FromNullable(TPrimitive? value) => value is null ? default : TWrapper.From(value);
    }    
}