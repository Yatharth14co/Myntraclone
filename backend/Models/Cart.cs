namespace ECommerceApi.Models;

/// <summary>
/// Represents a shopping cart
/// </summary>
public class Cart
{
    public int Id { get; set; }
    public List<CartItem> Items { get; set; } = new List<CartItem>();
    public string? CouponCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Calculates cart subtotal before discount
    /// </summary>
    public decimal GetSubtotal() => Items.Sum(item => item.Quantity * item.UnitPrice);
}
