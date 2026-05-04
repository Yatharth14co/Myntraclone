using Xunit;
using Moq;
using ECommerceApi.Services;
using ECommerceApi.Repositories;
using ECommerceApi.Models;
using ECommerceApi.Exceptions;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;

namespace ECommerceApi.Tests;

/// <summary>
/// Unit tests for coupon service
/// </summary>
public class CouponServiceTests
{
    private readonly Mock<ICouponRepository> _mockCouponRepository;
    private readonly Mock<IConnectionMultiplexer> _mockRedis;
    private readonly Mock<ILogger<CouponService>> _mockLogger;
    private readonly CouponService _couponService;

    public CouponServiceTests()
    {
        _mockCouponRepository = new Mock<ICouponRepository>();
        _mockRedis = new Mock<IConnectionMultiplexer>();
        _mockLogger = new Mock<ILogger<CouponService>>();

        // Mock Redis database
        var mockDb = new Mock<IDatabase>();
        _mockRedis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(mockDb.Object);

        _couponService = new CouponService(_mockCouponRepository.Object, _mockRedis.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task ValidateAndGetCouponAsync_WithValidFlatCoupon_ReturnsCorrectDiscount()
    {
        // Arrange
        var coupon = new Coupon
        {
            Code = "FLAT50",
            Type = CouponType.Flat,
            Value = 50m,
            MinimumCartValue = 500m,
            IsActive = true,
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };

        _mockCouponRepository.Setup(r => r.GetCouponByCodeAsync("FLAT50"))
            .ReturnsAsync(coupon);

        var mockDb = new Mock<IDatabase>();
        mockDb.Setup(d => d.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);
        mockDb.Setup(d => d.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        _mockRedis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(mockDb.Object);

        // Act
        var result = await _couponService.ValidateAndGetCouponAsync("FLAT50", 1000m);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("FLAT50", result.Code);
        Assert.Equal(50m, result.DiscountAmount);
    }

    [Fact]
    public async Task ValidateAndGetCouponAsync_WithExpiredCoupon_ThrowsInvalidCouponException()
    {
        // Arrange
        var coupon = new Coupon
        {
            Code = "EXPIRED",
            Type = CouponType.Flat,
            Value = 50m,
            MinimumCartValue = 500m,
            IsActive = true,
            ExpiresAt = DateTime.UtcNow.AddDays(-1)
        };

        _mockCouponRepository.Setup(r => r.GetCouponByCodeAsync("EXPIRED"))
            .ReturnsAsync(coupon);

        var mockDb = new Mock<IDatabase>();
        mockDb.Setup(d => d.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        _mockRedis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(mockDb.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidCouponException>(
            () => _couponService.ValidateAndGetCouponAsync("EXPIRED", 1000m));
    }

    [Fact]
    public async Task ValidateAndGetCouponAsync_WithInsufficientCartValue_ThrowsInvalidCouponException()
    {
        // Arrange
        var coupon = new Coupon
        {
            Code = "FLAT50",
            Type = CouponType.Flat,
            Value = 50m,
            MinimumCartValue = 500m,
            IsActive = true,
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };

        _mockCouponRepository.Setup(r => r.GetCouponByCodeAsync("FLAT50"))
            .ReturnsAsync(coupon);

        var mockDb = new Mock<IDatabase>();
        mockDb.Setup(d => d.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        _mockRedis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(mockDb.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidCouponException>(
            () => _couponService.ValidateAndGetCouponAsync("FLAT50", 300m));
    }

    [Fact]
    public async Task ValidateAndGetCouponAsync_WithPercentageCoupon_ReturnsCorrectDiscount()
    {
        // Arrange
        var coupon = new Coupon
        {
            Code = "SAVE10",
            Type = CouponType.Percentage,
            Value = 10m,
            MaxDiscount = 200m,
            MinimumCartValue = 1000m,
            IsActive = true,
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };

        _mockCouponRepository.Setup(r => r.GetCouponByCodeAsync("SAVE10"))
            .ReturnsAsync(coupon);

        var mockDb = new Mock<IDatabase>();
        mockDb.Setup(d => d.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);
        mockDb.Setup(d => d.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        _mockRedis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(mockDb.Object);

        // Act
        var result = await _couponService.ValidateAndGetCouponAsync("SAVE10", 2000m);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("SAVE10", result.Code);
        // 10% of 2000 = 200, but max discount is 200, so should be 200
        Assert.Equal(200m, result.DiscountAmount);
    }

    [Fact]
    public async Task CalculateDiscountAsync_WithValidCoupon_ReturnsCorrectAmount()
    {
        // Arrange
        var coupon = new Coupon
        {
            Code = "FLAT50",
            Type = CouponType.Flat,
            Value = 50m,
            MinimumCartValue = 500m,
            IsActive = true,
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };

        _mockCouponRepository.Setup(r => r.GetCouponByCodeAsync("FLAT50"))
            .ReturnsAsync(coupon);

        var mockDb = new Mock<IDatabase>();
        mockDb.Setup(d => d.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);
        mockDb.Setup(d => d.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        _mockRedis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(mockDb.Object);

        // Act
        var result = await _couponService.CalculateDiscountAsync("FLAT50", 1000m);

        // Assert
        Assert.Equal(50m, result);
    }
}
