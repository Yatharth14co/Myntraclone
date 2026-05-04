# Testing Guide

## Backend Testing

### Running Unit Tests

```bash
cd backend
dotnet test
```

### Running Specific Test Project

```bash
dotnet test Tests/CartServiceTests.cs
dotnet test Tests/OrderServiceTests.cs
dotnet test Tests/CouponServiceTests.cs
```

### Test Coverage

```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=lcov
```

### Writing Tests

Tests are organized by feature:
- `CartServiceTests.cs` - Cart management tests
- `CouponServiceTests.cs` - Coupon validation tests
- `OrderServiceTests.cs` - Order processing tests

Example test structure:
```csharp
[Fact]
public async Task AddToCart_ValidProduct_ReturnsSuccess()
{
    // Arrange
    var cartService = new CartService(mockRepository);
    
    // Act
    var result = await cartService.AddItemAsync(productId, quantity);
    
    // Assert
    Assert.True(result.Success);
}
```

## Frontend Testing

### Running Tests

```bash
cd frontend
npm run test
```

### Writing Component Tests

```typescript
import { render, screen } from '@testing-library/react';
import ProductCard from './ProductCard';

test('renders product details', () => {
  const product = { id: 1, name: 'Test', price: 100 };
  render(<ProductCard product={product} />);
  expect(screen.getByText('Test')).toBeInTheDocument();
});
```

## Integration Testing

### Manual Testing Steps

1. **Add Product to Cart**
   - Navigate to products page
   - Click "Add to Cart"
   - Verify cart updates

2. **Apply Coupon**
   - Add coupon code
   - Verify discount applied
   - Check price calculation

3. **Checkout**
   - Complete checkout flow
   - Verify order created
   - Check confirmation

### Automated Integration Tests

```bash
cd backend
dotnet test --filter "Category=Integration"
```

## Performance Testing

### Load Testing with Apache JMeter

1. Open JMeter
2. Create test plan with:
   - Thread Group (10 users)
   - HTTP Request samplers
   - Results listeners

### Profiling

```bash
# Backend profiling
dotnet run --collect="cpu_samples"
```

## Test Data

### Seeding Test Data

Database seeding happens automatically on startup. See `DataSeeder.cs` for initial data.

### Manual Test Data

Create test users and products via API:
```bash
curl -X POST http://localhost:5000/api/products \
  -H "Content-Type: application/json" \
  -d '{"name":"Test Product","price":99.99,"stock":100}'
```

## Continuous Integration

Tests run automatically on:
- Pull requests
- Commits to main branch
- Release builds

See `.github/workflows/ci-cd.yml` for CI configuration.

## Coverage Goals

- Backend: Target 80%+ code coverage
- Frontend: Target 70%+ component coverage
- Critical paths: 100% coverage

## Debugging Tests

### Backend
```bash
# Run with verbose output
dotnet test --verbosity=detailed

# Debug specific test
dotnet test --filter "MethodName" --logger "console;verbosity=detailed"
```

### Frontend
```bash
# Run in watch mode
npm run test -- --watch

# Debug in browser
npm run test -- --debug
```
