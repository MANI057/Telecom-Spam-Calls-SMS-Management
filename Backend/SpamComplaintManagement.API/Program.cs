using Microsoft.EntityFrameworkCore;
using SpamComplaintManagement.Business.Services;
using SpamComplaintManagement.Data.Data;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// Database
// ----------------------------------------------------

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "DefaultConnection was not found in appsettings.json.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// ----------------------------------------------------
// Controllers
// ----------------------------------------------------

builder.Services.AddControllers();

// ----------------------------------------------------
// Business Services
// ----------------------------------------------------

builder.Services.AddScoped<IAccountService, AccountService>();

// Register your other services here when they exist.
// Example:
// builder.Services.AddScoped<IComplaintService, ComplaintService>();

// ----------------------------------------------------
// Swagger
// ----------------------------------------------------

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// ----------------------------------------------------
// CORS
// ----------------------------------------------------

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ----------------------------------------------------
// Build application
// ----------------------------------------------------

var app = builder.Build();

// ----------------------------------------------------
// Development tools
// ----------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

// ----------------------------------------------------
// Middleware
// ----------------------------------------------------

app.UseCors("AllowFrontend");

app.UseAuthorization();

// ----------------------------------------------------
// Controllers
// ----------------------------------------------------

app.MapControllers();

// ----------------------------------------------------
// Database initialization
// ----------------------------------------------------

// Do NOT use EnsureCreated() here if you are using
// EF Core migrations.
//
// Database/migration creation should be handled using:
// dotnet ef database update

app.Run();