using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using ECommerceApi.DTOs;
using ECommerceApi.Services;

namespace ECommerceApi.Controllers;

/// <summary>
/// Products API endpoints
/// </summary>
[ApiController]
[Route("api/v1/products")]
[SwaggerTag("Products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Get all products with pagination and search
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term for product name or description</param>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all products", Description = "Returns paginated list of products")]
    [SwaggerResponse(200, "Products retrieved successfully")]
    [SwaggerResponse(400, "Invalid parameters")]
    public async Task<ActionResult<ApiResponse<PagedProductResponse>>> GetAllProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
    {
        try
        {
            var products = await _productService.GetProductsAsync(pageNumber, pageSize, searchTerm);
            return Ok(ApiResponse<PagedProductResponse>.SuccessResponse(products, "Products retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching products");
            return BadRequest(ApiResponse<PagedProductResponse>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get product by ID", Description = "Returns a specific product")]
    [SwaggerResponse(200, "Product retrieved successfully")]
    [SwaggerResponse(404, "Product not found")]
    public async Task<ActionResult<ApiResponse<ProductResponse>>> GetProductById(int id)
    {
        if (id <= 0)
            return BadRequest(ApiResponse<ProductResponse>.ErrorResponse("Product ID must be greater than 0"));

        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound(ApiResponse<ProductResponse>.ErrorResponse("Product not found"));

            return Ok(ApiResponse<ProductResponse>.SuccessResponse(product, "Product retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product {ProductId}", id);
            return BadRequest(ApiResponse<ProductResponse>.ErrorResponse(ex.Message));
        }
    }
}
