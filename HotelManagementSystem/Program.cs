using HotelManagementSystem.Controllers.CategoryController;
using HotelManagementSystem.DLL.AssignWaiterDLL;
using HotelManagementSystem.DLL.BillDLL;
using HotelManagementSystem.DLL.CategoryDLL;
using HotelManagementSystem.DLL.DinningDLL;
using HotelManagementSystem.DLL.InventoryDLL;
using HotelManagementSystem.DLL.MenuDLL;
using HotelManagementSystem.DLL.OrderDLL;
using HotelManagementSystem.DLL.OrderItemDLL;
using HotelManagementSystem.DLL.RecipeDLL;
using HotelManagementSystem.DLL.ReportDLL;
using HotelManagementSystem.DLL.Tables;
using HotelManagementSystem.DLL.UnitDLL;
using HotelManagementSystem.DLL.Users;
using HotelManagementSystem.Helper.JWT;
using HotelManagementSystem.Interfaces.BillInterface;
using HotelManagementSystem.Interfaces.CategoryInterface;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.DinningInterface;
using HotelManagementSystem.Interfaces.Inventory;
using HotelManagementSystem.Interfaces.JWTInterface;
using HotelManagementSystem.Interfaces.MenuInterface;
using HotelManagementSystem.Interfaces.OrderInterface;
using HotelManagementSystem.Interfaces.OrderItemInterface;
using HotelManagementSystem.Interfaces.RecipeInterface;
using HotelManagementSystem.Interfaces.Report;
using HotelManagementSystem.Interfaces.SubCategoryInterface;
using HotelManagementSystem.Interfaces.TableInterface;
using HotelManagementSystem.Interfaces.Units;
using HotelManagementSystem.Interfaces.User;
using HotelManagementSystem.Interfaces.UserInterfaces;
using HotelManagementSystem.Services.BillService;
using HotelManagementSystem.Services.Categories;
using HotelManagementSystem.Services.CategoryService;
using HotelManagementSystem.Services.Dinning;
using HotelManagementSystem.Services.Inventory;
using HotelManagementSystem.Services.MenuService;
using HotelManagementSystem.Services.OrderService;
using HotelManagementSystem.Services.RecipeService;
using HotelManagementSystem.Services.Report;
using HotelManagementSystem.Services.Table;
using HotelManagementSystem.Services.Units;
using HotelManagementSystem.Services.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Reflection;
using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using NSwag.Generation.Processors.Security;
using NSwag;
using HotelManagementSystem.Interfaces.PaymentInterface;
using HotelManagementSystem.Services.PaymentService;
using HotelManagementSystem.DLL.PaymentDLL;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IJWT, JWT>();

// For DLL layer
builder.Services.AddScoped<IUserDLL, UserDLL>();
builder.Services.AddScoped<ITableDLL, TableDLL>();
builder.Services.AddScoped<IDinningDLL, DinningDLL>();
builder.Services.AddScoped<IWaiterDLL, AssignWaiterDLL>();
builder.Services.AddScoped<IUnitDLL, UnitDLL>();
builder.Services.AddScoped<ICategoryDLL, CategoryDLL>();
builder.Services.AddScoped<IMenuDLL, MenuDLL>();
builder.Services.AddScoped<IInventoryDLL, InventoryDLL>();
builder.Services.AddScoped<IRecipeDLL, RecipeDLL>();
// Register SubCategory data layer implementation
builder.Services.AddScoped<HotelManagementSystem.Interfaces.SubCategoryInterface.ISubCategoryDLL,
                           HotelManagementSystem.Controllers.CategoryController.SubCategoryController>();
builder.Services.AddScoped<IOrderDLL, OrderDLL>();
builder.Services.AddScoped<IOrderItemDLL, OrderItemDLL>();
builder.Services.AddScoped<IBillDLL, BillDLL>();
builder.Services.AddScoped<IReportDLL, ReportDLL>();
builder.Services.AddScoped<IPaymentDLL, PaymentDLL>();

// For Service layer
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
// Register BillService as a scoped service and enable IHttpClientFactory so
// the service can create HttpClient instances without being registered as a
// typed HttpClient. This ensures other scoped dependencies (like IPaymentDLL)
// are resolved correctly.
builder.Services.AddScoped<IBillService, BillService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddControllers();

builder.Services.AddOpenApiDocument(options =>
{
    options.Title = "Hotel Management API";
    options.Version = "V1";

    options.AddSecurity("Bearer",  new OpenApiSecurityScheme
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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        )
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllLocal", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAllLocal");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.UseStaticFiles();


app.MapGet("/", () => "Hello World!");

app.Run();

public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _configuration;

    
    public SqlConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateConnection()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        return new SqlConnection(connectionString);
    }
}
