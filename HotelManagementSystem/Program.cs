using HotelManagementSystem.DLL.Users;
using HotelManagementSystem.Interfaces.DatabaseConnection;
using HotelManagementSystem.Interfaces.UserInterfaces;
using HotelManagementSystem.Services.User;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
// using Scalar.AspNetCore; // removed to avoid source-generator incompatibility causing CS0200
using System.Data;

var builder = WebApplication.CreateBuilder(args);


// Services
builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();

        
//for dll
builder.Services.AddScoped<IUserDLL, UserDLL>();

//for service layer
builder.Services.AddScoped<IUserService, UserServices>();







builder.Services.AddControllers();

// Swagger / OpenAPI (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
