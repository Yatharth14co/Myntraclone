namespace ECommerceApi.Exceptions;

/// <summary>
/// Exception thrown when a business rule is violated
/// </summary>
public class BusinessException : Exception
{
    public string? ErrorCode { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }

    public BusinessException(string message, string? errorCode = null, Dictionary<string, string[]>? errors = null)
        : base(message)
    {
        ErrorCode = errorCode;
        Errors = errors;
    }
}

/// <summary>
/// Exception thrown when a resource is not found
/// </summary>
public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when stock is insufficient
/// </summary>
public class InsufficientStockException : BusinessException
{
    public int RequestedQuantity { get; set; }
    public int AvailableQuantity { get; set; }

    public InsufficientStockException(int requested, int available)
        : base($"Insufficient stock. Requested: {requested}, Available: {available}", "INSUFFICIENT_STOCK")
    {
        RequestedQuantity = requested;
        AvailableQuantity = available;
    }
}

/// <summary>
/// Exception thrown when coupon is invalid or expired
/// </summary>
public class InvalidCouponException : BusinessException
{
    public InvalidCouponException(string message) : base(message, "INVALID_COUPON") { }
}

/// <summary>
/// Exception thrown when checkout fails due to concurrency issues
/// </summary>
public class CheckoutException : BusinessException
{
    public CheckoutException(string message) : base(message, "CHECKOUT_FAILED") { }
}

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public class ValidationException : BusinessException
{
    public ValidationException(string message, Dictionary<string, string[]> errors)
        : base(message, "VALIDATION_ERROR", errors) { }
}
