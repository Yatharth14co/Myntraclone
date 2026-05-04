using Xunit;
using Moq;
using ECommerceApi.Services;
using ECommerceApi.Repositories;
using ECommerceApi.Models;
using ECommerceApi.Exceptions;
using ECommerceApi.Data;
using Microsoft.Extensions.Logging;

namespace ECommerceApi.Tests;

/// <summary>
/// Unit tests for order service
/// </summary>
public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<ICartRepository> _mockCartRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ICouponService> _mockCouponService;
    private readonly Mock<ApplicationDbContext> _mockDbContext;
    private readonly Mock<ILogger<OrderService>> _mockLogger;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockCartRepository = new Mock<ICartRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCouponService = new Mock<ICouponService>();
        _mockDbContext = new Mock<ApplicationDbContext>(new object());
        _mockLogger = new Mock<ILogger<OrderService>>();

        // Mock database transaction
        var mockTransaction = new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>();
        _mockDbContext.Setup(d => d.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockTransaction.Object);

        _orderService = new OrderService(
            _mockOrderRepository.Object,
            _mockCartRepository.Object,
            _mockProductRepository.Object,
            _mockCouponService.Object,
            _mockDbContext.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task CheckoutAsync_WithValidCart_CreatesOrderSuccessfully()
    {
        // Arrange
        var cart = new Cart
        {
            Id = 1,
            Items = new List<CartItem>
            {
                new CartItem
                {
                    Id = 1,
                    CartId = 1,
                    ProductId = 1,
                    ProductName = "Laptop Pro",
                    UnitPrice = 1299.99m,
                    Quantity = 1,
                    AddedAt = DateTime.UtcNow
                }
            },
            CouponCode = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var product = new Product
        {
            Id = 1,
            Name = "Laptop Pro",
            Price = 1299.99m,
            Stock = 50,
            Description = "High-performance laptop",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockCartRepository.Setup(r => r.GetCartByIdAsync(1))
            .ReturnsAsync(cart);

        _mockProductRepository.Setup(r => r.GetProductByIdAsync(1))
            .ReturnsAsync(product);

        _mockProductRepository.Setup(r => r.ReduceStockAsync(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        var createdOrder = new Order
        {
            Id = 1,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    Id = 1,
                    OrderId = 1,
                    ProductId = 1,
                    ProductName = "Laptop Pro",
                    UnitPrice = 1299.99m,
                    Quantity = 1,
                    LineTotal = 1299.99m
                }
            },
            Subtotal = 1299.99m,
            Discount = 0m,
            Tax = 233.99m,
            TotalAmount = 1533.98m,
            CouponCode = null,
            Status = OrderStatus.Confirmed,
            OrderedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockOrderRepository.Setup(r => r.CreateOrderAsync(It.IsAny<Order>()))
            .ReturnsAsync(createdOrder);

        _mockCartRepository.Setup(r => r.ClearCartAsync(1))
            .Returns(Task.CompletedTask);

        _mockCouponService.Setup(s => s.CalculateDiscountAsync(It.IsAny<string>(), It.IsAny<decimal>()))
            .ReturnsAsync(0m);

        var mockTransaction = new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>();
        _mockDbContext.Setup(d => d.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockTransaction.Object);

        // Act
        var result = await _orderService.CheckoutAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.OrderId);
        Assert.Equal(1, result.Items.Count);
        Assert.Equal(1299.99m, result.Subtotal);
    }

    [Fact]
    public async Task CheckoutAsync_WithEmptyCart_ThrowsCheckoutException()
    {
        // Arrange
        var cart = new Cart
        {
            Id = 1,
            Items = new List<CartItem>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockCartRepository.Setup(r => r.GetCartByIdAsync(1))
            .ReturnsAsync(cart);

        var mockTransaction = new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>();
        _mockDbContext.Setup(d => d.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockTransaction.Object);

        // Act & Assert
        await Assert.ThrowsAsync<CheckoutException>(() => _orderService.CheckoutAsync(1));
    }

    [Fact]
    public async Task CheckoutAsync_WithInsufficientStock_ThrowsInsufficientStockException()
    {
        // Arrange
        var cart = new Cart
        {
            Id = 1,
            Items = new List<CartItem>
            {
                new CartItem
                {
                    Id = 1,
                    CartId = 1,
                    ProductId = 1,
                    ProductName = "Laptop Pro",
                    UnitPrice = 1299.99m,
                    Quantity = 100,
                    AddedAt = DateTime.UtcNow
                }
            },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var product = new Product
        {
            Id = 1,
            Name = "Laptop Pro",
            Price = 1299.99m,
            Stock = 10,
            Description = "High-performance laptop",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockCartRepository.Setup(r => r.GetCartByIdAsync(1))
            .ReturnsAsync(cart);

        _mockProductRepository.Setup(r => r.GetProductByIdAsync(1))
            .ReturnsAsync(product);

        var mockTransaction = new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>();
        _mockDbContext.Setup(d => d.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockTransaction.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InsufficientStockException>(() => _orderService.CheckoutAsync(1));
    }

    [Fact]
    public async Task GetOrderAsync_WithValidOrderId_ReturnsOrderSummary()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    Id = 1,
                    OrderId = 1,
                    ProductId = 1,
                    ProductName = "Laptop Pro",
                    UnitPrice = 1299.99m,
                    Quantity = 1,
                    LineTotal = 1299.99m
                }
            },
            Subtotal = 1299.99m,
            Discount = 0m,
            Tax = 233.99m,
            TotalAmount = 1533.98m,
            Status = OrderStatus.Confirmed,
            OrderedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockOrderRepository.Setup(r => r.GetOrderByIdAsync(1))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.GetOrderAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.OrderId);
        Assert.Equal(1299.99m, result.Subtotal);
    }
}
