using Microsoft.EntityFrameworkCore;
using FluentValidation;
using StackExchange.Redis;
using ECommerceApi.Data;
using ECommerceApi.Services;
using ECommerceApi.Repositories;
using ECommerceApi.Validators;
using ECommerceApi.Middleware;
using ECommerceApi.DTOs;

var builder = WebApplicationBuilder.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "E-Commerce API",
        Version = "v1",
        Description = "Smart Cart & Coupon Checkout System API",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "API Support",
            Email = "support@ecommerce.com"
        }
    });

    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "ECommerceApi.xml"));
    options.EnableAnnotations();
});

// Configure Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "InMemory";

if (connectionString == "InMemory")
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("ECommerceDb"));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// Configure Redis
var redisConnection = builder.Configuration.GetConnectionString("Redis")
    ?? "localhost:6379";

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    try
    {
        return ConnectionMultiplexer.Connect(redisConnection);
    }
    catch
    {
        // Log warning and continue without Redis
        Console.WriteLine($"Warning: Could not connect to Redis at {redisConnection}. Caching will be disabled.");
        return null!;
    }
});

// Register Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();

// Register Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Register Validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateCartItemRequestValidator>();

// Configure Rate Limiting
builder.Services.AddCustomRateLimiting();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Configure Logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
    DataSeeder.SeedData(dbContext);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Commerce API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Use rate limiting and exception handling middleware
app.UseRateLimiter();
app.UseExceptionHandling();

app.UseAuthorization();

app.MapControllers();

app.Run();
