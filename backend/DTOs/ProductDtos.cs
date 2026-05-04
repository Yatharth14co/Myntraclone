namespace ECommerceApi.DTOs;

/// <summary>
/// DTO for product response
/// </summary>
public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// DTO for paginated product list
/// </summary>
public class PagedProductResponse
{
    public List<ProductResponse> Items { get; set; } = new List<ProductResponse>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
