namespace ECommerceApi.DTOs;

/// <summary>
/// DTO for order item response
/// </summary>
public class OrderItemResponse
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}

/// <summary>
/// DTO for checkout request
/// </summary>
public class CheckoutRequest
{
    public int CartId { get; set; }
}

/// <summary>
/// DTO for order confirmation response
/// </summary>
public class OrderConfirmationResponse
{
    public int OrderId { get; set; }
    public List<OrderItemResponse> Items { get; set; } = new List<OrderItemResponse>();
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal TotalAmount { get; set; }
    public string? CouponCode { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime OrderedAt { get; set; }
}

/// <summary>
/// DTO for order summary response
/// </summary>
public class OrderSummaryResponse
{
    public int OrderId { get; set; }
    public List<OrderItemResponse> Items { get; set; } = new List<OrderItemResponse>();
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime OrderedAt { get; set; }
}
