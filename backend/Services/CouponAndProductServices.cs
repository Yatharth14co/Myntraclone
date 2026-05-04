using ECommerceApi.DTOs;
using ECommerceApi.Data;
using ECommerceApi.Models;
using ECommerceApi.Repositories;
using ECommerceApi.Exceptions;
using StackExchange.Redis;

namespace ECommerceApi.Services;

/// <summary>
/// Interface for coupon service
/// </summary>
public interface ICouponService
{
    Task<CouponResponse?> ValidateAndGetCouponAsync(string couponCode, decimal cartSubtotal);
    Task<decimal> CalculateDiscountAsync(string couponCode, decimal subtotal);
}

/// <summary>
/// Coupon service implementation
/// </summary>
public class CouponService : ICouponService
{
    private readonly ICouponRepository _repository;
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<CouponService> _logger;
    private const string COUPON_CACHE_KEY = "coupon:";
    private const int CACHE_EXPIRY_MINUTES = 60;

    public CouponService(ICouponRepository repository, IConnectionMultiplexer redis, ILogger<CouponService> logger)
    {
        _repository = repository;
        _redis = redis;
        _logger = logger;
    }

    public async Task<CouponResponse?> ValidateAndGetCouponAsync(string couponCode, decimal cartSubtotal)
    {
        if (string.IsNullOrWhiteSpace(couponCode))
            return null;

        var cacheKey = $"{COUPON_CACHE_KEY}{couponCode.ToUpper()}";
        var db = _redis.GetDatabase();
        var cachedCoupon = await db.StringGetAsync(cacheKey);

        Coupon? coupon = null;

        if (cachedCoupon.HasValue)
        {
            _logger.LogInformation("Coupon {Code} retrieved from cache", couponCode);
            coupon = System.Text.Json.JsonSerializer.Deserialize<Coupon>(cachedCoupon.ToString());
        }
        else
        {
            coupon = await _repository.GetCouponByCodeAsync(couponCode);
            if (coupon != null)
            {
                var json = System.Text.Json.JsonSerializer.Serialize(coupon);
                await db.StringSetAsync(cacheKey, json, TimeSpan.FromMinutes(CACHE_EXPIRY_MINUTES));
                _logger.LogInformation("Coupon {Code} cached", couponCode);
            }
        }

        if (coupon == null || !coupon.IsActive)
            throw new InvalidCouponException("Coupon code is invalid or inactive");

        if (coupon.ExpiresAt.HasValue && coupon.ExpiresAt < DateTime.UtcNow)
            throw new InvalidCouponException("Coupon has expired");

        if (cartSubtotal < coupon.MinimumCartValue)
            throw new InvalidCouponException($"Coupon is valid only for cart value >= {coupon.MinimumCartValue}");

        var discount = CalculateDiscount(coupon, cartSubtotal);

        return new CouponResponse
        {
            Code = coupon.Code,
            DiscountAmount = discount,
            Description = GetCouponDescription(coupon)
        };
    }

    public async Task<decimal> CalculateDiscountAsync(string couponCode, decimal subtotal)
    {
        var couponResponse = await ValidateAndGetCouponAsync(couponCode, subtotal);
        return couponResponse?.DiscountAmount ?? 0m;
    }

    private decimal CalculateDiscount(Coupon coupon, decimal subtotal)
    {
        decimal discount = 0m;

        if (coupon.Type == CouponType.Flat)
        {
            discount = coupon.Value;
        }
        else if (coupon.Type == CouponType.Percentage)
        {
            discount = (subtotal * coupon.Value) / 100m;
            
            if (coupon.MaxDiscount.HasValue && discount > coupon.MaxDiscount)
            {
                discount = coupon.MaxDiscount.Value;
            }
        }

        return Math.Min(discount, subtotal);
    }

    private string GetCouponDescription(Coupon coupon)
    {
        if (coupon.Type == CouponType.Flat)
            return $"Flat ₹{coupon.Value} discount";
        else
            return $"{coupon.Value}% discount" + (coupon.MaxDiscount.HasValue ? $" (max ₹{coupon.MaxDiscount})" : "");
    }
}

/// <summary>
/// Interface for product service
/// </summary>
public interface IProductService
{
    Task<PagedProductResponse> GetProductsAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null);
    Task<ProductResponse?> GetProductByIdAsync(int id);
}

/// <summary>
/// Product service implementation
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository repository, ILogger<ProductService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedProductResponse> GetProductsAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var products = await _repository.GetAllProductsAsync(pageNumber, pageSize, searchTerm);
        var totalCount = await _repository.GetTotalProductCountAsync(searchTerm);

        var productResponses = products.Select(p => new ProductResponse
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Stock = p.Stock,
            Description = p.Description
        }).ToList();

        var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

        return new PagedProductResponse
        {
            Items = productResponses,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    public async Task<ProductResponse?> GetProductByIdAsync(int id)
    {
        var product = await _repository.GetProductByIdAsync(id);
        if (product == null)
            return null;

        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            Description = product.Description
        };
    }
}
