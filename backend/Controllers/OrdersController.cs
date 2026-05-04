using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using ECommerceApi.DTOs;
using ECommerceApi.Services;
using ECommerceApi.Exceptions;

namespace ECommerceApi.Controllers;

/// <summary>
/// Orders API endpoints
/// </summary>
[ApiController]
[Route("api/v1/orders")]
[SwaggerTag("Orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IOrderService orderService,
        ICartService cartService,
        ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _cartService = cartService;
        _logger = logger;
    }

    /// <summary>
    /// Checkout and create order
    /// </summary>
    /// <param name="cartId">Cart ID</param>
    [HttpPost("checkout/{cartId}")]
    [SwaggerOperation(Summary = "Checkout", Description = "Creates order from cart, reduces stock, and applies discount")]
    [SwaggerResponse(200, "Order created successfully")]
    [SwaggerResponse(400, "Invalid cart or insufficient stock")]
    [SwaggerResponse(404, "Cart not found")]
    [SwaggerResponse(429, "Too many requests")]
    [EnableRateLimiting("checkout")]
    public async Task<ActionResult<ApiResponse<OrderConfirmationResponse>>> Checkout(int cartId)
    {
        if (cartId <= 0)
            return BadRequest(ApiResponse<OrderConfirmationResponse>.ErrorResponse("Cart ID must be greater than 0"));

        try
        {
            var order = await _orderService.CheckoutAsync(cartId);
            _logger.LogInformation("Checkout successful for cart {CartId}, order {OrderId}", cartId, order.OrderId);
            return Ok(ApiResponse<OrderConfirmationResponse>.SuccessResponse(order, "Order created successfully"));
        }
        catch (ResourceNotFoundException ex)
        {
            return NotFound(ApiResponse<OrderConfirmationResponse>.ErrorResponse(ex.Message));
        }
        catch (InsufficientStockException ex)
        {
            return BadRequest(ApiResponse<OrderConfirmationResponse>.ErrorResponse(
                $"Cannot complete checkout: {ex.Message}"));
        }
        catch (CheckoutException ex)
        {
            return BadRequest(ApiResponse<OrderConfirmationResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during checkout for cart {CartId}", cartId);
            return BadRequest(ApiResponse<OrderConfirmationResponse>.ErrorResponse(
                "Checkout failed. Please try again."));
        }
    }

    /// <summary>
    /// Get order details
    /// </summary>
    /// <param name="orderId">Order ID</param>
    [HttpGet("{orderId}")]
    [SwaggerOperation(Summary = "Get order", Description = "Returns order summary with items and pricing")]
    [SwaggerResponse(200, "Order retrieved successfully")]
    [SwaggerResponse(404, "Order not found")]
    public async Task<ActionResult<ApiResponse<OrderSummaryResponse>>> GetOrder(int orderId)
    {
        if (orderId <= 0)
            return BadRequest(ApiResponse<OrderSummaryResponse>.ErrorResponse("Order ID must be greater than 0"));

        try
        {
            var order = await _orderService.GetOrderAsync(orderId);
            if (order == null)
                return NotFound(ApiResponse<OrderSummaryResponse>.ErrorResponse("Order not found"));

            return Ok(ApiResponse<OrderSummaryResponse>.SuccessResponse(order, "Order retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching order {OrderId}", orderId);
            return BadRequest(ApiResponse<OrderSummaryResponse>.ErrorResponse(ex.Message));
        }
    }
}

/// <summary>
/// Cart Management API endpoints
/// </summary>
[ApiController]
[Route("api/v1/carts")]
[SwaggerTag("Cart Management")]
public class CartsController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ILogger<CartsController> _logger;

    public CartsController(ICartService cartService, ILogger<CartsController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    /// <summary>
    /// Clear cart
    /// </summary>
    /// <param name="cartId">Cart ID</param>
    [HttpDelete("{cartId}")]
    [SwaggerOperation(Summary = "Clear cart", Description = "Removes all items from cart")]
    [SwaggerResponse(200, "Cart cleared successfully")]
    [SwaggerResponse(404, "Cart not found")]
    public async Task<ActionResult<ApiResponse>> ClearCart(int cartId)
    {
        if (cartId <= 0)
            return BadRequest(ApiResponse.ErrorResponse("Cart ID must be greater than 0"));

        try
        {
            await _cartService.RemoveCartAsync(cartId);
            return Ok(ApiResponse.SuccessResponse("Cart cleared successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cart {CartId}", cartId);
            return BadRequest(ApiResponse.ErrorResponse(ex.Message));
        }
    }
}
