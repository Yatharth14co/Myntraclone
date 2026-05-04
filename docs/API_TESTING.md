# API Testing Documentation

## Postman Collection

You can use this Postman collection to test all API endpoints.

### Base URL
```
http://localhost:5000/api/v1
```

### Create Cart
Before making requests, create a cart ID for testing.

## Endpoints

### 1. Get Products

**Request:**
```
GET /products?pageNumber=1&pageSize=10
```

**Curl:**
```bash
curl -X GET "http://localhost:5000/api/v1/products?pageNumber=1&pageSize=10" \
  -H "Content-Type: application/json"
```

### 2. Add to Cart

**Request:**
```
POST /cart/1/items
Content-Type: application/json

{
  "productId": 1,
  "quantity": 2
}
```

**Curl:**
```bash
curl -X POST "http://localhost:5000/api/v1/cart/1/items" \
  -H "Content-Type: application/json" \
  -d '{
    "productId": 1,
    "quantity": 2
  }'
```

### 3. Get Cart

**Request:**
```
GET /cart/1
```

**Curl:**
```bash
curl -X GET "http://localhost:5000/api/v1/cart/1" \
  -H "Content-Type: application/json"
```

### 4. Apply Coupon

**Request:**
```
POST /cart/1/apply-coupon
Content-Type: application/json

{
  "couponCode": "FLAT50"
}
```

**Curl:**
```bash
curl -X POST "http://localhost:5000/api/v1/cart/1/apply-coupon" \
  -H "Content-Type: application/json" \
  -d '{
    "couponCode": "FLAT50"
  }'
```

### 5. Checkout

**Request:**
```
POST /orders/checkout/1
```

**Curl:**
```bash
curl -X POST "http://localhost:5000/api/v1/orders/checkout/1" \
  -H "Content-Type: application/json"
```

### 6. Get Order

**Request:**
```
GET /orders/1
```

**Curl:**
```bash
curl -X GET "http://localhost:5000/api/v1/orders/1" \
  -H "Content-Type: application/json"
```

## Test Scenarios

### Scenario 1: Basic Purchase
1. Get all products
2. Add product (ID: 1) with quantity 1 to cart (ID: 1)
3. Proceed to checkout
4. Retrieve order details

### Scenario 2: Apply Coupon
1. Add products to cart
2. Apply coupon "FLAT50"
3. Verify discount is applied
4. Checkout with discount

### Scenario 3: Stock Validation
1. Try to add product with quantity exceeding stock
2. Should get error: "Insufficient stock"

### Scenario 4: Coupon Validation
1. Try to apply expired coupon
2. Try to apply coupon with insufficient cart value
3. Should get appropriate error messages

## Error Codes

| Status | Code | Message |
|--------|------|---------|
| 400 | VALIDATION_ERROR | Input validation failed |
| 400 | INSUFFICIENT_STOCK | Not enough stock available |
| 400 | INVALID_COUPON | Coupon is invalid or expired |
| 400 | CHECKOUT_FAILED | Checkout processing failed |
| 404 | NOT_FOUND | Resource not found |
| 429 | TOO_MANY_REQUESTS | Rate limit exceeded |
| 500 | INTERNAL_ERROR | Server error |
