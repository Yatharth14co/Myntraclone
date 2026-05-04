using Microsoft.EntityFrameworkCore;
using ECommerceApi.Data;
using ECommerceApi.Models;

namespace ECommerceApi.Repositories;

/// <summary>
/// Interface for product repository
/// </summary>
public interface IProductRepository
{
    Task<List<Product>> GetAllProductsAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null);
    Task<int> GetTotalProductCountAsync(string? searchTerm = null);
    Task<Product?> GetProductByIdAsync(int id);
    Task<bool> IsStockAvailableAsync(int productId, int quantity);
    Task ReduceStockAsync(int productId, int quantity);
    Task RestoreStockAsync(int productId, int quantity);
}

/// <summary>
/// Product repository implementation
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllProductsAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm));
        }

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<int> GetTotalProductCountAsync(string? searchTerm = null)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm));
        }

        return await query.CountAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> IsStockAvailableAsync(int productId, int quantity)
    {
        var product = await GetProductByIdAsync(productId);
        return product != null && product.Stock >= quantity;
    }

    public async Task ReduceStockAsync(int productId, int quantity)
    {
        var product = await GetProductByIdAsync(productId);
        if (product != null && product.Stock >= quantity)
        {
            product.Stock -= quantity;
            product.UpdatedAt = DateTime.UtcNow;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RestoreStockAsync(int productId, int quantity)
    {
        var product = await GetProductByIdAsync(productId);
        if (product != null)
        {
            product.Stock += quantity;
            product.UpdatedAt = DateTime.UtcNow;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}

/// <summary>
/// Interface for cart repository
/// </summary>
public interface ICartRepository
{
    Task<Cart?> GetCartByIdAsync(int cartId);
    Task<Cart> CreateCartAsync();
    Task<CartItem> AddOrUpdateCartItemAsync(int cartId, int productId, string productName, decimal unitPrice, int quantity);
    Task RemoveCartItemAsync(int cartItemId);
    Task ClearCartAsync(int cartId);
    Task UpdateCartCouponAsync(int cartId, string? couponCode);
}

/// <summary>
/// Cart repository implementation
/// </summary>
public class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _context;

    public CartRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetCartByIdAsync(int cartId)
    {
        return await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == cartId);
    }

    public async Task<Cart> CreateCartAsync()
    {
        var cart = new Cart
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();
        return cart;
    }

    public async Task<CartItem> AddOrUpdateCartItemAsync(int cartId, int productId, string productName, decimal unitPrice, int quantity)
    {
        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.Quantity = quantity;
            _context.CartItems.Update(existingItem);
        }
        else
        {
            var newItem = new CartItem
            {
                CartId = cartId,
                ProductId = productId,
                ProductName = productName,
                UnitPrice = unitPrice,
                Quantity = quantity,
                AddedAt = DateTime.UtcNow
            };

            _context.CartItems.Add(newItem);
            existingItem = newItem;
        }

        await _context.SaveChangesAsync();
        return existingItem;
    }

    public async Task RemoveCartItemAsync(int cartItemId)
    {
        var item = await _context.CartItems.FindAsync(cartItemId);
        if (item != null)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearCartAsync(int cartId)
    {
        var items = await _context.CartItems.Where(ci => ci.CartId == cartId).ToListAsync();
        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCartCouponAsync(int cartId, string? couponCode)
    {
        var cart = await GetCartByIdAsync(cartId);
        if (cart != null)
        {
            cart.CouponCode = couponCode;
            cart.UpdatedAt = DateTime.UtcNow;
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
        }
    }
}

/// <summary>
/// Interface for order repository
/// </summary>
public interface IOrderRepository
{
    Task<Order?> GetOrderByIdAsync(int orderId);
    Task<Order> CreateOrderAsync(Order order);
    Task<List<Order>> GetOrdersAsync(int pageNumber = 1, int pageSize = 10);
}

/// <summary>
/// Order repository implementation
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<List<Order>> GetOrdersAsync(int pageNumber = 1, int pageSize = 10)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .OrderByDescending(o => o.OrderedAt)
            .ToListAsync();
    }
}

/// <summary>
/// Interface for coupon repository
/// </summary>
public interface ICouponRepository
{
    Task<Coupon?> GetCouponByCodeAsync(string code);
    Task<List<Coupon>> GetActiveCouponsAsync();
}

/// <summary>
/// Coupon repository implementation
/// </summary>
public class CouponRepository : ICouponRepository
{
    private readonly ApplicationDbContext _context;

    public CouponRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Coupon?> GetCouponByCodeAsync(string code)
    {
        return await _context.Coupons
            .FirstOrDefaultAsync(c => c.Code == code.ToUpper());
    }

    public async Task<List<Coupon>> GetActiveCouponsAsync()
    {
        return await _context.Coupons
            .Where(c => c.IsActive && (c.ExpiresAt == null || c.ExpiresAt > DateTime.UtcNow))
            .ToListAsync();
    }
}
