namespace ECommerceApi.Models;

/// <summary>
/// Represents a coupon discount
/// </summary>
public class Coupon
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public CouponType Type { get; set; }
    public decimal Value { get; set; }
    public decimal? MaxDiscount { get; set; }
    public decimal MinimumCartValue { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public enum CouponType
{
    Flat = 0,    // Fixed amount discount
    Percentage = 1  // Percentage discount
}
