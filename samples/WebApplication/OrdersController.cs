using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private static readonly Order[] _orders =
    [
        new() { OrderId = OrderId.From(1), CustomerName = CustomerName.From("Fred")},
        new() { OrderId = OrderId.From(2), CustomerName = CustomerName.From("Barney")}
    ];

    [HttpGet]
    [Produces(typeof(IEnumerable<Order>))]
    public IActionResult CurrentOrders()
    {
        return Ok(_orders);
    }

    [HttpGet, Route("customer/{customerName}")]
    [Produces(typeof(IEnumerable<Order>))]
    public IActionResult GetByName(CustomerName customerName)
    {
        return Ok(_orders.Where(o => o.CustomerName == customerName));
    }

    [HttpGet, Route("{orderId}")]
    [Produces(typeof(Order))]
    public IActionResult GetByOrderId(OrderId orderId)
    {
        var order = _orders.SingleOrDefault(o => o.OrderId == orderId);
        if (order == null)
            return new NotFoundResult();
        return Ok(order);
    }
}

