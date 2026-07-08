using HotelManagementSystem.Controllers.CategoryController;
using HotelManagementSystem.DLL.AssignWaiterDLL;
using HotelManagementSystem.DLL.CategoryDLL;
using HotelManagementSystem.DLL.DinningDLL;
using HotelManagementSystem.DLL.InventoryDLL;
using HotelManagementSystem.DLL.MenuDLL;
using HotelManagementSystem.DLL.RecipeDLL;
using HotelManagementSystem.DLL.Tables;
using HotelManagementSystem.DLL.UnitDLL;
using HotelManagementSystem.DLL.Users;
using HotelManagementSystem.Helper.JWT;
using HotelManagementSystem.Interfaces.CategoryInterface;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.DinningInterface;
using HotelManagementSystem.Interfaces.Inventory;
using HotelManagementSystem.Interfaces.JWTInterface;
using HotelManagementSystem.Interfaces.MenuInterface;
using HotelManagementSystem.Interfaces.RecipeInterface;
using HotelManagementSystem.Interfaces.SubCategoryInterface;
using HotelManagementSystem.Interfaces.TableInterface;
using HotelManagementSystem.Interfaces.Units;
using HotelManagementSystem.Interfaces.User;
using HotelManagementSystem.Interfaces.UserInterfaces;
using HotelManagementSystem.Models.Categories;
using HotelManagementSystem.Services.Categories;
using HotelManagementSystem.Services.CategoryService;
using HotelManagementSystem.Services.Categories;
using HotelManagementSystem.Services.Dinning;
using HotelManagementSystem.Services.Inventory;
using HotelManagementSystem.Services.MenuService;
using HotelManagementSystem.Services.Table;
using HotelManagementSystem.Services.Units;
using HotelManagementSystem.Services.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
// using Scalar.AspNetCore; // removed to avoid source-generator incompatibility causing CS0200
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using HotelManagementSystem.Services.RecipeService;

var builder = WebApplication.CreateBuilder(args);


// Services
builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IJWT, JWT>();

        
//for dll
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


//for service layer
builder.Services.AddScoped<IUserService, UserServices>();
builder.Services.AddScoped<ITableService, TableService>();
builder.Services.AddScoped<IDinningService, DinningService>();
builder.Services.AddScoped<IUnitServices, UnitServices>();
builder.Services.AddScoped<IInventoryService, InventoryServices>();
builder.Services.AddScoped<IMenuServices, MenuService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();
// SubCategory DLL implementation not present; register the service implementation
builder.Services.AddScoped<ISubCategoryService, SubCategoryServices>();



builder.Services.AddControllers();

// Swagger / OpenAPI (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAllLocal");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();

public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("Connection string not found.");
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
