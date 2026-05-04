namespace ECommerceApi.Models;

/// <summary>
/// Represents a placed order
/// </summary>
public class Order
{
    public int Id { get; set; }
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal TotalAmount { get; set; }
    public string? CouponCode { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime OrderedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}
