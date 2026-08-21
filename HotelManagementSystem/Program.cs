using HotelManagementSystem.Controllers.CategoryController;
using HotelManagementSystem.DLL.AssignWaiterDLL;
using HotelManagementSystem.DLL.BillDLL;
using HotelManagementSystem.DLL.CartDLL;
using HotelManagementSystem.DLL.CategoryDLL;
using HotelManagementSystem.DLL.DinningDLL;
using HotelManagementSystem.DLL.InventoryDLL;
using HotelManagementSystem.DLL.MenuDLL;
using HotelManagementSystem.DLL.OrderDLL;
using HotelManagementSystem.DLL.OrderItemDLL;
using HotelManagementSystem.DLL.PaymentDLL;
using HotelManagementSystem.DLL.RecipeDLL;
using HotelManagementSystem.DLL.ReportDLL;
using HotelManagementSystem.DLL.RolesDLL;
using HotelManagementSystem.DLL.Tables;
using HotelManagementSystem.DLL.UnitDLL;
using HotelManagementSystem.DLL.Users;

using HotelManagementSystem.Helper.JWT;
using HotelManagementSystem.Helper.RequestLoggingMiddleware;
using HotelManagementSystem.Hubs;

using HotelManagementSystem.Interfaces;
using HotelManagementSystem.Interfaces.BillInterface;
using HotelManagementSystem.Interfaces.CategoryInterface;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.DinningInterface;
using HotelManagementSystem.Interfaces.EmailInterface;
using HotelManagementSystem.Interfaces.Inventory;
using HotelManagementSystem.Interfaces.JWTInterface;
using HotelManagementSystem.Interfaces.MenuInterface;
using HotelManagementSystem.Interfaces.OrderInterface;
using HotelManagementSystem.Interfaces.OrderItemInterface;
using HotelManagementSystem.Interfaces.PaymentInterface;
using HotelManagementSystem.Interfaces.RecipeInterface;
using HotelManagementSystem.Interfaces.Report;
using HotelManagementSystem.Interfaces.Roles;
using HotelManagementSystem.Interfaces.SubCategoryInterface;
using HotelManagementSystem.Interfaces.TableInterface;
using HotelManagementSystem.Interfaces.Units;
using HotelManagementSystem.Interfaces.User;
using HotelManagementSystem.Interfaces.UserInterfaces;
using HotelManagementSystem.Interfaces.Redis;

using HotelManagementSystem.Services.BillService;
using HotelManagementSystem.Services.CartService;
using HotelManagementSystem.Services.Categories;
using HotelManagementSystem.Services.CategoryService;
using HotelManagementSystem.Services.Dinning;
using HotelManagementSystem.Services.Email;
using HotelManagementSystem.Services.Inventory;
using HotelManagementSystem.Services.MenuService;
using HotelManagementSystem.Services.OrderService;
using HotelManagementSystem.Services.PaymentService;
using HotelManagementSystem.Services.RecipeService;
using HotelManagementSystem.Services.Report;
using HotelManagementSystem.Services.Roles;
using HotelManagementSystem.Services.Table;
using HotelManagementSystem.Services.Units;
using HotelManagementSystem.Services.User;
using HotelManagementSystem.Services.Redis;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

using NSwag;
using NSwag.Generation.Processors.Security;

using StackExchange.Redis;

using System.Data;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var connectionString =
        builder.Configuration.GetConnectionString("Redis")
        ?? throw new InvalidOperationException(
            "Redis connection string not found.");

    return ConnectionMultiplexer.Connect(connectionString);
});

builder.Services.AddScoped<IRedisService, RedisService>();

builder.Services.AddScoped<IJWT, JWT>();

builder.Services.AddScoped<IUserDLL, UserDLL>();
builder.Services.AddScoped<ITableDLL, TableDLL>();
builder.Services.AddScoped<IDinningDLL, DinningDLL>();
builder.Services.AddScoped<IWaiterDLL, AssignWaiterDLL>();
builder.Services.AddScoped<IUnitDLL, UnitDLL>();
builder.Services.AddScoped<ICategoryDLL, CategoryDLL>();
builder.Services.AddScoped<IMenuDLL, MenuDLL>();
builder.Services.AddScoped<IInventoryDLL, InventoryDLL>();
builder.Services.AddScoped<IRecipeDLL, RecipeDLL>();
builder.Services.AddScoped<ISubCategoryDLL, SubCategoryController>();
builder.Services.AddScoped<IOrderDLL, OrderDLL>();
builder.Services.AddScoped<IOrderItemDLL, OrderItemDLL>();
builder.Services.AddScoped<IBillDLL, BillDLL>();
builder.Services.AddScoped<IReportDLL, ReportDLL>();
builder.Services.AddScoped<IPaymentDLL, PaymentDLL>();
builder.Services.AddScoped<ICartDLL, CartDLL>();
builder.Services.AddScoped<IRoleDLL, RolesDLL>();

builder.Services.AddScoped<IUserService, UserServices>();
builder.Services.AddScoped<ITableService, TableService>();
builder.Services.AddScoped<IDinningService, DinningService>();
builder.Services.AddScoped<IUnitServices, UnitServices>();
builder.Services.AddScoped<IInventoryService, InventoryServices>();
builder.Services.AddScoped<IMenuServices, MenuService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<ISubCategoryService, SubCategoryServices>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IRoleService, RolesService>();
builder.Services.AddScoped<IBillService, BillService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddHttpClient();

builder.Services.AddControllers();

builder.Services.AddOpenApiDocument(options =>
{
    options.Title = "Hotel Management API";
    options.Version = "V1";

    options.AddSecurity("Bearer", new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Enter your JWT token."
    });

    options.OperationProcessors.Add(
        new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!
                        )
                    )
            };
    });

builder.Services.AddAuthorization();

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter =
        PartitionedRateLimiter.Create<HttpContext, string>(
            httpContext =>
            {
                var ipAddress =
                    httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: ipAddress,
                    factory: _ =>
                        new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromSeconds(10),
                            QueueLimit = 0
                        });
            });

    options.RejectionStatusCode =
        StatusCodes.Status429TooManyRequests;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllLocal", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

var logger =
    app.Services.GetRequiredService<ILogger<Program>>();

app.Use(async (context, next) =>
{
    logger.LogInformation(
        "1. Processing Request: {Method} {Path}",
        context.Request.Method,
        context.Request.Path);

    await next();

    Console.WriteLine(
        "this is just the test middleware");

    logger.LogInformation(
        "2. Processing Response: {StatusCode}",
        context.Response.StatusCode);
});

app.Use(async (context, next) =>
{
    Console.WriteLine("1. Processing Request");

    await next();

    Console.WriteLine("2. Processing Response");
});

app.UseRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAllLocal");

app.UseStaticFiles();

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<OrderHub>("/hubs/orders");

app.MapFallbackToFile("index.html");

app.MapGet("/", () => "Hello World!");

app.Run();

public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _configuration;

    public SqlConnectionFactory(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateConnection()
    {
        var connectionString =
            _configuration.GetConnectionString(
                "DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found.");

        return new SqlConnection(connectionString);
    }
}