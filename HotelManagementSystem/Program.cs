using HotelManagementSystem.DLL.AssignWaiterDLL;
using HotelManagementSystem.DLL.DinningDLL;
using HotelManagementSystem.DLL.Tables;
using HotelManagementSystem.DLL.Users;
using HotelManagementSystem.Helper.JWT;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.DinningInterface;
using HotelManagementSystem.Interfaces.JWTInterface;
using HotelManagementSystem.Interfaces.TableInterface;
using HotelManagementSystem.Interfaces.User;
using HotelManagementSystem.Interfaces.UserInterfaces;
using HotelManagementSystem.Services.Dinning;
using HotelManagementSystem.Services.Table;
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

var builder = WebApplication.CreateBuilder(args);


// Services
builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IJWT, JWT>();

        
//for dll
builder.Services.AddScoped<IUserDLL, UserDLL>();
builder.Services.AddScoped<ITableDLL, TableDLL>();
builder.Services.AddScoped<IDinningDLL, DinningDLL>();
builder.Services.AddScoped<IWaiterDLL, AssignWaiterDLL>();
//for service layer
builder.Services.AddScoped<IUserService, UserServices>();
builder.Services.AddScoped<ITableService, TableService>();
builder.Services.AddScoped<IDinningService, DinningService>();








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
