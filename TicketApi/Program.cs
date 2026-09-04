using TicketAPI.Services;
using TicketAPI.Data;
using Microsoft.EntityFrameworkCore;
using TicketAPI.Middleware;
using Scalar.AspNetCore;
using TicketAPI.Interfaces;
using TicketAPI.Data.EntityFramework;
using System.Data;
using Microsoft.Data.SqlClient;
using TicketAPI.Data.Dapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketQueryRepository, TicketQueryRepository>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//Conexão utilizada pelo EntityFramework
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

//Conexão utilizada pelo Dapper
builder.Services.AddScoped<IDbConnection>(_ => 
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); 
    app.MapScalarApiReference(); 
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
