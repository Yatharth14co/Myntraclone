using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;
using ECommerceApi.DTOs;
using ECommerceApi.Services;
using ECommerceApi.Exceptions;

namespace ECommerceApi.Controllers;

/// <summary>
/// Shopping Cart API endpoints
/// </summary>
[ApiController]
[Route("api/v1/cart")]
[SwaggerTag("Cart")]
[EnableRateLimiting("cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly IValidator<CreateCartItemRequest> _createCartItemValidator;
    private readonly IValidator<ApplyCouponRequest> _applyCouponValidator;
    private readonly ILogger<CartController> _logger;

    public CartController(
        ICartService cartService,
        IValidator<CreateCartItemRequest> createCartItemValidator,
        IValidator<ApplyCouponRequest> applyCouponValidator,
        ILogger<CartController> logger)
    {
        _cartService = cartService;
        _createCartItemValidator = createCartItemValidator;
        _applyCouponValidator = applyCouponValidator;
        _logger = logger;
    }

    /// <summary>
    /// Get cart details
    /// </summary>
    /// <param name="cartId">Cart ID</param>
    [HttpGet("{cartId}")]
    [SwaggerOperation(Summary = "Get cart details", Description = "Returns cart with items and pricing")]
    [SwaggerResponse(200, "Cart retrieved successfully")]
    [SwaggerResponse(404, "Cart not found")]
    public async Task<ActionResult<ApiResponse<CartResponse>>> GetCart(int cartId)
    {
        if (cartId <= 0)
            return BadRequest(ApiResponse<CartResponse>.ErrorResponse("Cart ID must be greater than 0"));

        try
        {
            var cart = await _cartService.GetCartAsync(cartId);
            return Ok(ApiResponse<CartResponse>.SuccessResponse(cart, "Cart retrieved successfully"));
        }
        catch (ResourceNotFoundException ex)
        {
            return NotFound(ApiResponse<CartResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching cart {CartId}", cartId);
            return BadRequest(ApiResponse<CartResponse>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Add or update item in cart
    /// </summary>
    /// <param name="cartId">Cart ID</param>
    /// <param name="request">Cart item details</param>
    [HttpPost("{cartId}/items")]
    [SwaggerOperation(Summary = "Add or update cart item", Description = "Adds a new item or updates quantity if item exists")]
    [SwaggerResponse(200, "Item added/updated successfully")]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(404, "Cart or Product not found")]
    public async Task<ActionResult<ApiResponse<CartResponse>>> AddOrUpdateCartItem(
        int cartId,
        [FromBody] CreateCartItemRequest request)
    {
        if (cartId <= 0)
            return BadRequest(ApiResponse<CartResponse>.ErrorResponse("Cart ID must be greater than 0"));

        var validationResult = await _createCartItemValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return BadRequest(ApiResponse<CartResponse>.ErrorResponse(
                "Validation failed",
                errors));
        }

        try
        {
            var cart = await _cartService.AddItemToCartAsync(cartId, request);
            return Ok(ApiResponse<CartResponse>.SuccessResponse(cart, "Item added to cart successfully"));
        }
        catch (ResourceNotFoundException ex)
        {
            return NotFound(ApiResponse<CartResponse>.ErrorResponse(ex.Message));
        }
        catch (InsufficientStockException ex)
        {
            return BadRequest(ApiResponse<CartResponse>.ErrorResponse(
                $"Insufficient stock: {ex.Message}"));
        }
        catch (BusinessException ex)
        {
            return BadRequest(ApiResponse<CartResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to cart {CartId}", cartId);
            return BadRequest(ApiResponse<CartResponse>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Apply coupon to cart
    /// </summary>
    /// <param name="cartId">Cart ID</param>
    /// <param name="request">Coupon code</param>
    [HttpPost("{cartId}/apply-coupon")]
    [SwaggerOperation(Summary = "Apply coupon", Description = "Validates and applies coupon code to cart")]
    [SwaggerResponse(200, "Coupon applied successfully")]
    [SwaggerResponse(400, "Invalid coupon or request")]
    [SwaggerResponse(404, "Cart not found")]
    public async Task<ActionResult<ApiResponse<CartResponse>>> ApplyCoupon(
        int cartId,
        [FromBody] ApplyCouponRequest request)
    {
        if (cartId <= 0)
            return BadRequest(ApiResponse<CartResponse>.ErrorResponse("Cart ID must be greater than 0"));

        var validationResult = await _applyCouponValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return BadRequest(ApiResponse<CartResponse>.ErrorResponse(
                "Validation failed",
                errors));
        }

        try
        {
            var cart = await _cartService.ApplyCouponAsync(cartId, request.CouponCode);
            return Ok(ApiResponse<CartResponse>.SuccessResponse(cart, "Coupon applied successfully"));
        }
        catch (ResourceNotFoundException ex)
        {
            return NotFound(ApiResponse<CartResponse>.ErrorResponse(ex.Message));
        }
        catch (InvalidCouponException ex)
        {
            return BadRequest(ApiResponse<CartResponse>.ErrorResponse(ex.Message));
        }
        catch (BusinessException ex)
        {
            return BadRequest(ApiResponse<CartResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying coupon to cart {CartId}", cartId);
            return BadRequest(ApiResponse<CartResponse>.ErrorResponse(ex.Message));
        }
    }
}
