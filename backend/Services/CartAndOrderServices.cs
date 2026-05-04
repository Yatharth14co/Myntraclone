using ECommerceApi.DTOs;
using ECommerceApi.Models;
using ECommerceApi.Repositories;
using ECommerceApi.Exceptions;
using ECommerceApi.Data;

namespace ECommerceApi.Services;

/// <summary>
/// Interface for cart service
/// </summary>
public interface ICartService
{
    Task<CartResponse> GetCartAsync(int cartId);
    Task<CartResponse> AddItemToCartAsync(int cartId, CreateCartItemRequest request);
    Task<CartResponse> ApplyCouponAsync(int cartId, string couponCode);
    Task RemoveCartAsync(int cartId);
}

/// <summary>
/// Cart service implementation with business logic
/// </summary>
public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICouponService _couponService;
    private readonly ILogger<CartService> _logger;

    public CartService(
        ICartRepository cartRepository,
        IProductRepository productRepository,
        ICouponService couponService,
        ILogger<CartService> logger)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _couponService = couponService;
        _logger = logger;
    }

    public async Task<CartResponse> GetCartAsync(int cartId)
    {
        var cart = await _cartRepository.GetCartByIdAsync(cartId)
            ?? throw new ResourceNotFoundException($"Cart with ID {cartId} not found");

        return MapToCartResponse(cart);
    }

    public async Task<CartResponse> AddItemToCartAsync(int cartId, CreateCartItemRequest request)
    {
        if (request.Quantity <= 0)
            throw new BusinessException("Quantity must be greater than 0");

        var cart = await _cartRepository.GetCartByIdAsync(cartId)
            ?? throw new ResourceNotFoundException($"Cart with ID {cartId} not found");

        var product = await _productRepository.GetProductByIdAsync(request.ProductId)
            ?? throw new ResourceNotFoundException($"Product with ID {request.ProductId} not found");

        if (!await _productRepository.IsStockAvailableAsync(request.ProductId, request.Quantity))
            throw new InsufficientStockException(request.Quantity, product.Stock);

        await _cartRepository.AddOrUpdateCartItemAsync(
            cartId,
            request.ProductId,
            product.Name,
            product.Price,
            request.Quantity);

        _logger.LogInformation("Item {ProductId} added to cart {CartId}", request.ProductId, cartId);

        return await GetCartAsync(cartId);
    }

    public async Task<CartResponse> ApplyCouponAsync(int cartId, string couponCode)
    {
        var cart = await _cartRepository.GetCartByIdAsync(cartId)
            ?? throw new ResourceNotFoundException($"Cart with ID {cartId} not found");

        if (cart.Items.Count == 0)
            throw new BusinessException("Cannot apply coupon to empty cart");

        var subtotal = cart.GetSubtotal();
        var coupon = await _couponService.ValidateAndGetCouponAsync(couponCode, subtotal);

        if (coupon == null)
            throw new InvalidCouponException("Coupon is not valid");

        await _cartRepository.UpdateCartCouponAsync(cartId, coupon.Code);

        _logger.LogInformation("Coupon {Code} applied to cart {CartId}", coupon.Code, cartId);

        return await GetCartAsync(cartId);
    }

    public async Task RemoveCartAsync(int cartId)
    {
        await _cartRepository.ClearCartAsync(cartId);
        _logger.LogInformation("Cart {CartId} cleared", cartId);
    }

    private CartResponse MapToCartResponse(Cart cart)
    {
        var subtotal = cart.GetSubtotal();
        var discount = 0m;

        if (!string.IsNullOrWhiteSpace(cart.CouponCode))
        {
            try
            {
                // Calculate discount asynchronously but synchronously in this context
                // In production, consider refactoring
                var discountTask = _couponService.CalculateDiscountAsync(cart.CouponCode, subtotal);
                discountTask.Wait();
                discount = discountTask.Result;
            }
            catch
            {
                _logger.LogWarning("Failed to calculate discount for coupon {Code}", cart.CouponCode);
            }
        }

        var cartItems = cart.Items.Select(ci => new CartItemResponse
        {
            Id = ci.Id,
            ProductId = ci.ProductId,
            ProductName = ci.ProductName,
            UnitPrice = ci.UnitPrice,
            Quantity = ci.Quantity,
            LineTotal = ci.Quantity * ci.UnitPrice
        }).ToList();

        return new CartResponse
        {
            Id = cart.Id,
            Items = cartItems,
            CouponCode = cart.CouponCode,
            Subtotal = subtotal,
            Discount = discount,
            Total = subtotal - discount
        };
    }
}

/// <summary>
/// Interface for order service
/// </summary>
public interface IOrderService
{
    Task<OrderConfirmationResponse> CheckoutAsync(int cartId);
    Task<OrderSummaryResponse?> GetOrderAsync(int orderId);
}

/// <summary>
/// Order service implementation with atomic transaction handling
/// </summary>
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICouponService _couponService;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IProductRepository productRepository,
        ICouponService couponService,
        ApplicationDbContext dbContext,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _couponService = couponService;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<OrderConfirmationResponse> CheckoutAsync(int cartId)
    {
        var cart = await _cartRepository.GetCartByIdAsync(cartId)
            ?? throw new ResourceNotFoundException($"Cart with ID {cartId} not found");

        if (cart.Items.Count == 0)
            throw new CheckoutException("Cannot checkout empty cart");

        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            // Validate stock availability before creating order
            foreach (var item in cart.Items)
            {
                var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                if (product == null || product.Stock < item.Quantity)
                {
                    throw new InsufficientStockException(
                        item.Quantity,
                        product?.Stock ?? 0);
                }
            }

            var subtotal = cart.GetSubtotal();
            var discount = 0m;

            if (!string.IsNullOrWhiteSpace(cart.CouponCode))
            {
                discount = await _couponService.CalculateDiscountAsync(cart.CouponCode, subtotal);
            }

            const decimal taxRate = 0.18m; // 18% GST
            var taxableAmount = subtotal - discount;
            var tax = taxableAmount * taxRate;
            var total = taxableAmount + tax;

            // Create order
            var order = new Order
            {
                Subtotal = subtotal,
                Discount = discount,
                Tax = tax,
                TotalAmount = total,
                CouponCode = cart.CouponCode,
                Status = OrderStatus.Confirmed,
                OrderedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add order items and reduce stock
            foreach (var cartItem in cart.Items)
            {
                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.ProductName,
                    UnitPrice = cartItem.UnitPrice,
                    Quantity = cartItem.Quantity,
                    LineTotal = cartItem.Quantity * cartItem.UnitPrice
                };

                order.Items.Add(orderItem);

                // Reduce stock
                await _productRepository.ReduceStockAsync(cartItem.ProductId, cartItem.Quantity);
            }

            // Save order
            var createdOrder = await _orderRepository.CreateOrderAsync(order);

            // Clear cart
            await _cartRepository.ClearCartAsync(cartId);

            await transaction.CommitAsync();

            _logger.LogInformation("Order {OrderId} created successfully for cart {CartId}", createdOrder.Id, cartId);

            return MapToOrderConfirmationResponse(createdOrder);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Checkout failed for cart {CartId}", cartId);
            throw new CheckoutException($"Checkout failed: {ex.Message}");
        }
    }

    public async Task<OrderSummaryResponse?> GetOrderAsync(int orderId)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order == null)
            return null;

        return MapToOrderSummaryResponse(order);
    }

    private OrderConfirmationResponse MapToOrderConfirmationResponse(Order order)
    {
        return new OrderConfirmationResponse
        {
            OrderId = order.Id,
            Items = order.Items.Select(oi => new OrderItemResponse
            {
                ProductId = oi.ProductId,
                ProductName = oi.ProductName,
                UnitPrice = oi.UnitPrice,
                Quantity = oi.Quantity,
                LineTotal = oi.LineTotal
            }).ToList(),
            Subtotal = order.Subtotal,
            Discount = order.Discount,
            Tax = order.Tax,
            TotalAmount = order.TotalAmount,
            CouponCode = order.CouponCode,
            Status = order.Status.ToString(),
            OrderedAt = order.OrderedAt
        };
    }

    private OrderSummaryResponse MapToOrderSummaryResponse(Order order)
    {
        return new OrderSummaryResponse
        {
            OrderId = order.Id,
            Items = order.Items.Select(oi => new OrderItemResponse
            {
                ProductId = oi.ProductId,
                ProductName = oi.ProductName,
                UnitPrice = oi.UnitPrice,
                Quantity = oi.Quantity,
                LineTotal = oi.LineTotal
            }).ToList(),
            Subtotal = order.Subtotal,
            Discount = order.Discount,
            Tax = order.Tax,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            OrderedAt = order.OrderedAt
        };
    }
}
