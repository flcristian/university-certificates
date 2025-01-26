using System.Text.Json.Serialization;
using DotNetEnv;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UniversityCertificates.Certificates.Repository;
using UniversityCertificates.Certificates.Repository.Interfaces;
using UniversityCertificates.Certificates.Services;
using UniversityCertificates.Certificates.Services.Interfaces;
using UniversityCertificates.Data;
using UniversityCertificates.Register.Repository;
using UniversityCertificates.Register.Repository.Interfaces;
using UniversityCertificates.Register.Services;
using UniversityCertificates.Register.Services.Interfaces;
using UniversityCertificates.Students.Repository;
using UniversityCertificates.Students.Repository.Interfaces;
using UniversityCertificates.Students.Services;
using UniversityCertificates.Students.Services.Interfaces;
using UniversityCertificates.System;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(typeof(MappingProfile));

#region SERVICES

builder.Services.AddScoped<IStudentsRepository, StudentsRepository>();
builder.Services.AddScoped<IStudentsQueryService, StudentsQueryService>();
builder.Services.AddScoped<IStudentsCommandService, StudentsCommandService>();

builder.Services.AddScoped<IRegisterEntriesRepository, RegisterEntriesRepository>();
builder.Services.AddScoped<IRegisterEntriesQueryService, RegisterEntriesQueryService>();
builder.Services.AddScoped<IRegisterEntriesCommandService, RegisterEntriesCommandService>();
builder.Services.AddScoped<IRegisterEntryQRCodesService, RegisterEntryQRCodesService>();
builder.Services.AddScoped<IRegisterEntryDocumentsService, RegisterEntryDocumentsService>();

builder.Services.AddScoped<ICertificateTemplatesRepository, CertificateTemplatesRepository>();
builder.Services.AddScoped<
    ICertificateTemplateFilesRepository,
    CertificateTemplateFilesRepository
>();
builder.Services.AddScoped<ICertificateTemplatesQueryService, CertificateTemplatesQueryService>();
builder.Services.AddScoped<
    ICertificateTemplatesCommandService,
    CertificateTemplatesCommandService
>();

#endregion

builder.Services.AddLogging();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "University Certificates API", Version = "v1" });
});

string envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (!File.Exists(envPath))
{
    Console.WriteLine($"Configuration Error: .env file not found at {envPath}");
    return;
}

Env.Load();

builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(8080);
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddCors(options =>
    options.AddPolicy(
        "university-certificates",
        domain => domain.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod()
    )
);

string connectionString =
    $"Server={Env.GetString("DB_HOST")};"
    + $"Port=3306;"
    + $"Database={Env.GetString("DB_NAME")};"
    + $"Uid={Env.GetString("DB_USER")};"
    + $"Pwd={Env.GetString("DB_PASSWORD")};";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

builder
    .Services.AddFluentMigratorCore()
    .ConfigureRunner(rb =>
        rb.AddMySql8()
            .WithGlobalConnectionString(connectionString)
            .ScanIn(typeof(Program).Assembly)
            .For.Migrations()
    )
    .AddLogging(lb => lb.AddFluentMigratorConsole());

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "University Certificates API v1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

using (IServiceScope scope = app.Services.CreateScope())
{
    IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}

app.UseCors("university-certificates");
app.Run();
