using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using UniversityCertificates.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
    options.AddPolicy("university-certificates", domain => domain.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod())
);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

Env.Load();

var connectionString = $"Server={Env.GetString("DB_HOST")};" +
                      $"Database={Env.GetString("DB_NAME")};" +
                      $"User Id={Env.GetString("DB_USER")};" +
                      $"Password={Env.GetString("DB_PASSWORD")};" +
                      $"Port={Env.GetString("DB_PORT")}";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("university-certificates");
app.Run();
