using System.Text.Json;
using System.Text.Json.Serialization;
using Domain;
using Vogen;

namespace Infra;

public static class Program
{
        public static void Main()
        {
            Order o = new()
            {
                CustomerId = CustomerId.From(123),
                OrderId = OrderId.From(321),
                CustomerName = CustomerName.From("Fred")
            };
            
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                //    new Domain.VogenTypesFactory(),
                //    new Infra.VogenTypesFactory()
                }
            };

            var ctx = new SourceGenerationContext(options);

            var json = JsonSerializer.Serialize(o, ctx.Order);
            Order w2 = JsonSerializer.Deserialize(json, ctx.Order)!;
            //string s = JsonSerializer.Serialize(o, SourceGenerationContext.Default.Order);
            Console.WriteLine(json);
            Console.WriteLine(w2.CustomerId);
            Console.WriteLine(w2.OrderId);
            Console.WriteLine(w2.CustomerName);
        }
}



[ValueObject] public partial struct AnotherVo;
[ValueObject] public partial struct AnotherVo3;



[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Order))]
[JsonSerializable(typeof(int))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}

[EfCoreConverter<Domain.CustomerId>]
[EfCoreConverter<Domain.CustomerName>]
public partial class VogenEfCoreConverters;

