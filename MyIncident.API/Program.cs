using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MyIncident.API.Data;
using MyIncident.API.Middleware;
using MyIncident.API.Repositories;
using MyIncident.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database - PostgreSQL (Render)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IRequestService, RequestService>();

// CORS - allow Angular (local + Vercel)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "https://my-incident.vercel.app",
                "https://myincident.vercel.app"
              )
              .SetIsOriginAllowedToAllowWildcardSubdomains()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline FIRST (so port is open immediately)
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAngular");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

// Run DB initialization in background after app starts listening
app.Lifetime.ApplicationStarted.Register(() =>
{
    Task.Run(async () =>
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var creator = context.Database.GetService<Microsoft.EntityFrameworkCore.Storage.IRelationalDatabaseCreator>();
            if (!creator.HasTables())
            {
                logger.LogInformation("Creating database tables...");
                creator.CreateTables();
            }

            logger.LogInformation("Starting database seed...");
            await DatabaseSeeder.SeedAsync(context);
            logger.LogInformation("Database seed completed.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during database initialization");
        }
    });
});

app.Run();
