using Vogen;
[assembly: VogenDefaults(
    systemTextJsonConverterFactoryGeneration: SystemTextJsonConverterFactoryGeneration.Omit, 
    conversions: Conversions.SystemTextJson | 
                 Conversions.TypeConverter | 
                 Conversions.SystemTextJson)]

namespace Domain;

[ValueObject]
public partial struct CustomerId
{
}

[ValueObject<string>]
public partial struct CustomerName
{
}

[ValueObject]
public partial struct OrderId
{
}

public class Order
{
    public CustomerId CustomerId { get; set; }
    public OrderId OrderId { get; set; }
    public CustomerName CustomerName { get; set; }
}