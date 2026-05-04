namespace ECommerceApi.DTOs;

/// <summary>
/// DTO for creating/updating a cart item
/// </summary>
public class CreateCartItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

/// <summary>
/// DTO for cart item response
/// </summary>
public class CartItemResponse
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}

/// <summary>
/// DTO for cart response
/// </summary>
public class CartResponse
{
    public int Id { get; set; }
    public List<CartItemResponse> Items { get; set; } = new List<CartItemResponse>();
    public string? CouponCode { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
}

/// <summary>
/// DTO for applying coupon
/// </summary>
public class ApplyCouponRequest
{
    public string CouponCode { get; set; } = string.Empty;
}

/// <summary>
/// DTO for coupon response
/// </summary>
public class CouponResponse
{
    public string Code { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public string Description { get; set; } = string.Empty;
}
