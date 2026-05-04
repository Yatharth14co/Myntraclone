using Xunit;
using Moq;
using ECommerceApi.Services;
using ECommerceApi.Repositories;
using ECommerceApi.DTOs;
using ECommerceApi.Models;
using ECommerceApi.Exceptions;
using Microsoft.Extensions.Logging;

namespace ECommerceApi.Tests;

/// <summary>
/// Unit tests for cart service
/// </summary>
public class CartServiceTests
{
    private readonly Mock<ICartRepository> _mockCartRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ICouponService> _mockCouponService;
    private readonly Mock<ILogger<CartService>> _mockLogger;
    private readonly CartService _cartService;

    public CartServiceTests()
    {
        _mockCartRepository = new Mock<ICartRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCouponService = new Mock<ICouponService>();
        _mockLogger = new Mock<ILogger<CartService>>();

        _cartService = new CartService(
            _mockCartRepository.Object,
            _mockProductRepository.Object,
            _mockCouponService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task AddItemToCartAsync_WithValidProductAndQuantity_AddsItemToCart()
    {
        // Arrange
        var cartId = 1;
        var cart = new Cart
        {
            Id = cartId,
            Items = new List<CartItem>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var product = new Product
        {
            Id = 1,
            Name = "Wireless Mouse",
            Price = 29.99m,
            Stock = 100,
            Description = "Ergonomic wireless mouse",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var request = new CreateCartItemRequest { ProductId = 1, Quantity = 2 };

        _mockCartRepository.Setup(r => r.GetCartByIdAsync(cartId))
            .ReturnsAsync(cart);

        _mockProductRepository.Setup(r => r.GetProductByIdAsync(1))
            .ReturnsAsync(product);

        _mockProductRepository.Setup(r => r.IsStockAvailableAsync(1, 2))
            .ReturnsAsync(true);

        var cartItem = new CartItem
        {
            Id = 1,
            CartId = cartId,
            ProductId = 1,
            ProductName = "Wireless Mouse",
            UnitPrice = 29.99m,
            Quantity = 2,
            AddedAt = DateTime.UtcNow
        };

        _mockCartRepository.Setup(r => r.AddOrUpdateCartItemAsync(cartId, 1, "Wireless Mouse", 29.99m, 2))
            .ReturnsAsync(cartItem);

        var updatedCart = cart;
        updatedCart.Items.Add(cartItem);

        _mockCartRepository.Setup(r => r.GetCartByIdAsync(cartId))
            .ReturnsAsync(updatedCart);

        // Act
        var result = await _cartService.AddItemToCartAsync(cartId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cartId, result.Id);
        Assert.Single(result.Items);
        Assert.Equal(59.98m, result.Subtotal);
    }

    [Fact]
    public async Task AddItemToCartAsync_WithZeroQuantity_ThrowsBusinessException()
    {
        // Arrange
        var request = new CreateCartItemRequest { ProductId = 1, Quantity = 0 };

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _cartService.AddItemToCartAsync(1, request));
    }

    [Fact]
    public async Task AddItemToCartAsync_WithInsufficientStock_ThrowsInsufficientStockException()
    {
        // Arrange
        var cartId = 1;
        var cart = new Cart
        {
            Id = cartId,
            Items = new List<CartItem>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var product = new Product
        {
            Id = 1,
            Name = "Laptop Pro",
            Price = 1299.99m,
            Stock = 5,
            Description = "High-performance laptop",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var request = new CreateCartItemRequest { ProductId = 1, Quantity = 10 };

        _mockCartRepository.Setup(r => r.GetCartByIdAsync(cartId))
            .ReturnsAsync(cart);

        _mockProductRepository.Setup(r => r.GetProductByIdAsync(1))
            .ReturnsAsync(product);

        _mockProductRepository.Setup(r => r.IsStockAvailableAsync(1, 10))
            .ReturnsAsync(false);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InsufficientStockException>(() => _cartService.AddItemToCartAsync(cartId, request));
        Assert.Equal(10, ex.RequestedQuantity);
        Assert.Equal(5, ex.AvailableQuantity);
    }

    [Fact]
    public async Task GetCartAsync_WithValidCartId_ReturnsCartWithItems()
    {
        // Arrange
        var cartId = 1;
        var cart = new Cart
        {
            Id = cartId,
            Items = new List<CartItem>
            {
                new CartItem
                {
                    Id = 1,
                    CartId = cartId,
                    ProductId = 1,
                    ProductName = "Wireless Mouse",
                    UnitPrice = 29.99m,
                    Quantity = 2,
                    AddedAt = DateTime.UtcNow
                }
            },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockCartRepository.Setup(r => r.GetCartByIdAsync(cartId))
            .ReturnsAsync(cart);

        // Act
        var result = await _cartService.GetCartAsync(cartId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cartId, result.Id);
        Assert.Single(result.Items);
        Assert.Equal(59.98m, result.Subtotal);
    }

    [Fact]
    public async Task ApplyCouponAsync_WithEmptyCart_ThrowsBusinessException()
    {
        // Arrange
        var cartId = 1;
        var cart = new Cart
        {
            Id = cartId,
            Items = new List<CartItem>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockCartRepository.Setup(r => r.GetCartByIdAsync(cartId))
            .ReturnsAsync(cart);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _cartService.ApplyCouponAsync(cartId, "FLAT50"));
    }
}
