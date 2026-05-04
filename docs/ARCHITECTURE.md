# Architecture Documentation

## System Overview

### High-Level Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                        Client Browser                          │
└────────────┬──────────────────────────────────────────────┬─────┘
             │                                              │
             │ HTTP/HTTPS                                   │
             │                                              │
    ┌────────▼─────────┐                           ┌────────▼──────────┐
    │  React Frontend  │                           │  Swagger/OpenAPI  │
    │  (Port: 3000)    │                           │  (Port: 5000)     │
    └────────┬─────────┘                           └────────┬──────────┘
             │                                              │
             └──────────────────┬──────────────────────────┘
                                │
                    ┌───────────▼────────────┐
                    │   API Gateway Layer    │
                    │  (Rate Limiting, CORS) │
                    └───────────┬────────────┘
                                │
        ┌───────────────────────┼───────────────────────┐
        │                       │                       │
    ┌───▼─────────────┐   ┌────▼───────────┐  ┌────────▼─────┐
    │  Controllers    │   │  Middleware    │  │   Services   │
    │  (API Routes)   │   │  (Exception    │  │  (Business   │
    │                 │   │   Handling)    │  │   Logic)     │
    └───┬─────────────┘   └────┬───────────┘  └────────┬─────┘
        │                      │                       │
        └──────────────────────┼───────────────────────┘
                               │
                    ┌──────────▼──────────┐
                    │   Repositories     │
                    │   (Data Access)    │
                    └──────────┬─────────┘
                               │
        ┌──────────────────────┼──────────────────────┐
        │                      │                      │
    ┌───▼──────┐        ┌─────▼────┐         ┌──────▼──────┐
    │ In-Memory │        │  SQL     │         │    Redis    │
    │  DB      │        │ Server   │         │  (Cache)    │
    │(Dev)    │        │(Prod)    │         │             │
    └──────────┘        └──────────┘         └─────────────┘
```

## Backend Architecture Details

### Layer-by-Layer Breakdown

#### 1. **Presentation Layer (Controllers)**
- **File**: `Controllers/` directory
- **Responsibilities**:
  - Handle HTTP requests and responses
  - Route requests to appropriate services
  - Perform basic request validation
  - Return formatted API responses

**Key Controllers**:
- `ProductsController` - Product listing and retrieval
- `CartController` - Cart management operations
- `OrdersController` - Order creation and retrieval

#### 2. **Business Logic Layer (Services)**
- **File**: `Services/` directory
- **Responsibilities**:
  - Implement complex business rules
  - Coordinate between repositories
  - Handle transactions and state management
  - Perform caching strategies

**Key Services**:
- `ProductService` - Product filtering and pagination
- `CartService` - Cart operations and validations
- `CouponService` - Coupon validation and discount calculation
- `OrderService` - Order creation with atomic transactions

#### 3. **Data Access Layer (Repositories)**
- **File**: `Repositories/` directory
- **Responsibilities**:
  - Abstract database operations
  - Provide CRUD operations
  - Handle complex queries

**Key Repositories**:
- `ProductRepository` - Product CRUD and stock management
- `CartRepository` - Cart and cart item operations
- `OrderRepository` - Order persistence
- `CouponRepository` - Coupon lookup and retrieval

#### 4. **Data Models Layer**
- **File**: `Models/` directory
- **Entities**:
  - `Product` - Product catalog
  - `Cart` - Shopping cart
  - `CartItem` - Individual cart item
  - `Order` - Order record
  - `OrderItem` - Item in order
  - `Coupon` - Discount coupon

#### 5. **Cross-Cutting Concerns**
- **Middleware**: Request/response interception, exception handling
- **Validators**: Input validation using FluentValidation
- **Exceptions**: Custom exceptions for specific error scenarios
- **DTOs**: Data transfer objects for API contracts

### Database Schema

```
Products
├── Id (PK)
├── Name
├── Price
├── Stock
├── Description
└── Timestamps

Carts
├── Id (PK)
├── CouponCode (FK, nullable)
└── Items (Navigation)

CartItems
├── Id (PK)
├── CartId (FK)
├── ProductId
├── Quantity
└── UnitPrice

Orders
├── Id (PK)
├── Subtotal
├── Discount
├── Tax
├── TotalAmount
├── Status
└── Items (Navigation)

OrderItems
├── Id (PK)
├── OrderId (FK)
├── ProductId
└── Details

Coupons
├── Id (PK)
├── Code (Unique)
├── Type
├── Value
├── MaxDiscount
├── MinimumCartValue
├── IsActive
└── ExpiresAt
```

## Frontend Architecture Details

### Component Hierarchy

```
App
├── Navigation Bar
├── ProductsPage
│   ├── SearchBar
│   ├── ProductCard (× N)
│   │   ├── ProductImage
│   │   ├── ProductDetails
│   │   └── AddToCart Button
│   └── Pagination
└── CartPage
    ├── CartItems Table
    ├── CouponForm
    └── OrderSummary
        ├── PricingBreakdown
        └── CheckoutButton
```

### State Management (Zustand Store)

**State Structure**:
```typescript
{
  // Cart State
  cartId: number | null
  cart: Cart | null
  isCartLoading: boolean
  cartError: string | null

  // Products State
  products: Product[]
  isProductsLoading: boolean
  productsError: string | null
  currentPage: number
  totalPages: number

  // Order State
  order: OrderConfirmation | null
  isCheckingOut: boolean

  // UI State
  searchTerm: string
}
```

### Data Flow

```
User Interaction
    ↓
Component Event Handler
    ↓
Store Action (Zustand)
    ↓
API Service (Axios)
    ↓
Backend API
    ↓
Response Processing
    ↓
State Update
    ↓
Component Re-render
```

## Request/Response Flow

### Example: Add to Cart Flow

```
1. User clicks "Add to Cart"
   └─→ ProductCard component

2. ProductCard.handleAddToCart()
   └─→ store.addToCart(productId, quantity)

3. Store Action (Zustand)
   └─→ apiService.addToCart()

4. API Call (Axios)
   POST /api/v1/cart/1/items
   └─→ Backend

5. Backend Processing
   Controllers → Services → Repositories
   ├─ Validate product exists
   ├─ Check stock availability
   ├─ Update cart in database
   └─ Return updated cart

6. API Response
   200 OK with cart data

7. Store State Update
   └─→ set({ cart })

8. Component Re-render
   └─→ Display updated cart
```

## Security Layers

### Input Validation
- **Frontend**: React form validation
- **Backend**: FluentValidation (server-side)

### Rate Limiting
- Global: 100 requests/min per user
- Checkout: 10 requests/min (brute force prevention)
- Cart ops: 50 requests/min

### Error Handling
- Client errors: 4xx status codes
- Server errors: 500 status code
- Generic error messages to prevent information leakage

### Transaction Management
- Checkout operations wrapped in database transaction
- Atomicity guaranteed: all or nothing
- Rollback on any failure

## Caching Strategy

### Redis Cache
- **Coupon Caching**: 60-minute TTL
- **Cache Key**: `coupon:{code}`
- **Invalidation**: Manual through cache expiry

### Benefits
- Reduced database queries
- Faster coupon validation
- Improved response times

### Cache Flow
```
Request Coupon
    ↓
Check Cache
├─ Hit: Return cached coupon
└─ Miss: 
    ├─ Query database
    ├─ Store in cache (60 min)
    └─ Return coupon
```

## Performance Optimizations

### Backend
1. **Database Indexing**: On Code (Coupons), ProductId (Products)
2. **Pagination**: Limit query results
3. **Lazy Loading**: Include only necessary relationships
4. **Connection Pooling**: Reuse database connections
5. **Async/Await**: Non-blocking I/O operations

### Frontend
1. **Code Splitting**: Separate route components
2. **Memoization**: React.memo for ProductCard
3. **Lazy Loading**: Dynamic imports for routes
4. **CSS Optimization**: Tailwind CSS purging
5. **Image Optimization**: Compressed assets

## Deployment Architecture

### Docker Compose Setup

```
┌─────────────────────────────────────┐
│    Docker Compose Network            │
├──────────────────┬──────────────────┤
│                  │                  │
│ Backend Container│ Frontend Container
│ (Port 5000)      │ (Port 3000)      │
│                  │                  │
├──────────────────┴──────────────────┤
│  Redis Container (Port 6379)         │
└─────────────────────────────────────┘
```

### Environment Separation

**Development**
- In-Memory Database
- Debug logging enabled
- Hot reload enabled
- No caching

**Production**
- SQL Server Database
- Optimized logging
- Caching enabled
- Rate limiting active

## Scalability Considerations

### Horizontal Scaling
- Stateless API design
- Shared database
- Distributed caching (Redis Cluster)
- Load balancing (Nginx/HAProxy)

### Vertical Scaling
- Increase CPU/RAM on server
- Database optimization
- Query optimization

### Database Scaling
- Read replicas for queries
- Connection pooling
- Sharding for large datasets

## Monitoring & Observability

### Logging
- Structured logging (ILogger)
- Log levels: Debug, Information, Warning, Error
- Centralized logging ready

### Metrics
- Request/Response times
- Error rates
- Cache hit/miss ratios
- Database query performance

### Health Checks
- Database connectivity
- Redis connectivity
- API responsiveness

---

**Version**: 1.0  
**Last Updated**: May 4, 2024
