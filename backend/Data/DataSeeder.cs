using ECommerceApi.Models;

namespace ECommerceApi.Data;

/// <summary>
/// Seeds initial data into the database
/// </summary>
public static class DataSeeder
{
    public static void SeedData(ApplicationDbContext context)
    {
        if (context.Products.Any() || context.Coupons.Any())
            return;

        // Seed products
        var products = new List<Product>
        {
            new Product
            {
                Name = "Laptop Pro",
                Price = 1299.99m,
                Stock = 50,
                Description = "High-performance laptop with latest specs",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Wireless Mouse",
                Price = 29.99m,
                Stock = 200,
                Description = "Ergonomic wireless mouse with 2.4GHz connectivity",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "USB-C Hub",
                Price = 49.99m,
                Stock = 150,
                Description = "7-in-1 USB-C hub with multiple ports",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Mechanical Keyboard",
                Price = 159.99m,
                Stock = 80,
                Description = "RGB mechanical keyboard with custom switches",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "4K Monitor",
                Price = 599.99m,
                Stock = 40,
                Description = "27-inch 4K IPS display with USB-C connectivity",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Portable SSD",
                Price = 199.99m,
                Stock = 120,
                Description = "1TB portable SSD with fast transfer speeds",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Webcam HD",
                Price = 79.99m,
                Stock = 100,
                Description = "1080p HD webcam with built-in microphone",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Gaming Headset",
                Price = 129.99m,
                Stock = 90,
                Description = "7.1 surround sound gaming headset",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.Products.AddRange(products);

        // Seed coupons
        var coupons = new List<Coupon>
        {
            new Coupon
            {
                Code = "FLAT50",
                Type = CouponType.Flat,
                Value = 50m,
                MinimumCartValue = 500m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddYears(1)
            },
            new Coupon
            {
                Code = "SAVE10",
                Type = CouponType.Percentage,
                Value = 10m,
                MaxDiscount = 200m,
                MinimumCartValue = 1000m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddYears(1)
            },
            new Coupon
            {
                Code = "WELCOME20",
                Type = CouponType.Percentage,
                Value = 20m,
                MaxDiscount = 100m,
                MinimumCartValue = 100m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddYears(1)
            },
            new Coupon
            {
                Code = "FLASH100",
                Type = CouponType.Flat,
                Value = 100m,
                MinimumCartValue = 1500m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddYears(1)
            }
        };

        context.Coupons.AddRange(coupons);
        context.SaveChanges();
    }
}
