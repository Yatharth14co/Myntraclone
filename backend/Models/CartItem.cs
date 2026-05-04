namespace ECommerceApi.Models;

/// <summary>
/// Represents an item in a shopping cart
/// </summary>
public class CartItem
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public Cart? Cart { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public DateTime AddedAt { get; set; }
}
