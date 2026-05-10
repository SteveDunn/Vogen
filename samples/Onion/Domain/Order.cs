namespace Domain;

public class Order
{
    public required CustomerId CustomerId { get; set; }
    public required OrderId OrderId { get; set; }
    public required CustomerName CustomerName { get; set; }
}