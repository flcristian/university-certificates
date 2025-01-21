using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UniversityCertificates.Data;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(80);
});

builder.Services.AddCors(options =>
    options.AddPolicy("university-certificates", domain => domain.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod())
);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "University Certificates API", Version = "v1" });
});

ILogger logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
string envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (!File.Exists(envPath))
{
    logger.LogCritical($"Configuration Error: .env file not found at {envPath}");
    return;
}

Env.Load();

var connectionString = $"Server={Env.GetString("DB_HOST")};" +
                      $"Database={Env.GetString("DB_NAME")};" +
                      $"User Id={Env.GetString("DB_USER")};" +
                      $"Password={Env.GetString("DB_PASSWORD")};" +
                      $"Port={Env.GetString("DB_PORT")}";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "University Certificates API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("university-certificates");
app.Run();
