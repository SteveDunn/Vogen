namespace Domain;

public class Order
{
    public CustomerId CustomerId { get; set; }
    public OrderId OrderId { get; set; }
    public CustomerName CustomerName { get; set; }
}