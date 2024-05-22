using Microsoft.AspNetCore.Mvc;
using Vogen;

[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private static readonly Order[] _orders =
    [
        new() { OrderId = OrderId.From(1), CustomerName = CustomerName.From("Fred")},
        new() { OrderId = OrderId.From(2), CustomerName = CustomerName.From("Barney")}
    ];

    [HttpGet]
    public IActionResult CurrentOrders()
    {
        return Ok(_orders);
    }

    [HttpGet, Route("customer/{customerName}")]
    public IActionResult GetByName(CustomerName customerName)
    {
        return Ok(_orders.Where(o => o.CustomerName == customerName));
    }

    [HttpGet, Route("{orderId}")]
    public IActionResult GetByOrderId(OrderId orderId)
    {
        return Ok(_orders.Where(o => o.OrderId == orderId));
    }
}

[ValueObject<string>]
public partial class CustomerName
{
}

[ValueObject<int>]
public partial struct OrderId
{
}

public class Order
{
    public OrderId OrderId { get; init; } 

    public CustomerName CustomerName { get; init; } = CustomerName.From("");
}