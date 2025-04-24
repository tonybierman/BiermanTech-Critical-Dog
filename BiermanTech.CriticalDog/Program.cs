using AutoMapper;
using BiermanTech.CriticalDog;
using BiermanTech.CriticalDog.Authorization;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Reports;
using BiermanTech.CriticalDog.Services;
using BiermanTech.CriticalDog.Services.EntityServices;
using BiermanTech.CriticalDog.Services.Factories;
using BiermanTech.CriticalDog.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Serilog;
using System.Runtime.Intrinsics.X86;

var options = new WebApplicationOptions
{
    ContentRootPath = Path.GetFullPath(Directory.GetCurrentDirectory()),
    WebRootPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
};

var builder = WebApplication.CreateBuilder(options);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // Read from appsettings.json
    .Enrich.FromLogContext()
    .WriteTo.Console() // Console sink
    .WriteTo.File(
        path: "logs/criticaldog-.log", // File path with date-based rolling
        rollingInterval: RollingInterval.Day, // New file daily
        retainedFileCountLimit: 7, // Keep 7 days of logs
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}") // Detailed format
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddHttpContextAccessor();

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuthenticated", policy => 
        policy.RequireAuthenticatedUser());
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("SubjectCanView", policy =>
        policy.AddRequirements(new SubjectPermissionRequirement("CanView")));
    options.AddPolicy("SubjectCanEdit", policy =>
        policy.AddRequirements(new SubjectPermissionRequirement("CanEdit")));
    options.AddPolicy("SubjectCanDelete", policy =>
        policy.AddRequirements(new SubjectPermissionRequirement("CanDelete")));
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/"); // Require auth for all pages
    options.Conventions.AllowAnonymousToPage("/Index"); // Allow anonymous for /Pages/Index
    options.Conventions.AllowAnonymousToPage("/Subjects/Index"); // Allow anonymous for /Pages/Index
    options.Conventions.AllowAnonymousToPage("/Privacy"); // Allow anonymous for /Pages/Privacy
    options.Conventions.AllowAnonymousToFolder("/Reports"); // Allow anonymous for /Pages/Reports/*
    options.Conventions.AuthorizeFolder("/Admin", "RequireAdminRole"); // Require Admin role for /Pages/Admin/*

});

// Auto Mapper Configurations
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new DefaultMappingProfile());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Add services to the container.
builder.Services.AddScoped<ISelectListService, SelectListService>();
builder.Services.AddScoped<ISubjectObservationService, SubjectObservationService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IScientificDisciplineService, ScientificDisciplineService>();
builder.Services.AddScoped<IObservationDefinitionService, ObservationDefinitionService>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<IMetaTagService, MetaTagService>();
builder.Services.AddScoped<IMetricTypeService, MetricTypeService>();
builder.Services.AddScoped<IObservationTypeService, ObservationTypeService>();
builder.Services.AddScoped<ISubjectTypeService, SubjectTypeService>();
builder.Services.AddScoped<ISubjectRecordService, SubjectRecordService>();
builder.Services.AddScoped<IEnergyCalculationService, EnergyCalculationService>();
builder.Services.AddScoped<IDisciplineCardFactory, DisciplineCardFactory>();

// Auth
builder.Services.AddScoped<IAuthorizationHandler, SubjectPermissionHandler>();

// Analytics
builder.Services.AddScoped<IUnitConverter, UnitConverter>();
builder.Services.AddScoped<IObservationAnalyticsProvider, ObservationAnalyticsFactory>();

// Lob DB
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Identity DB
var identityConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__IdentityConnection")
    ?? builder.Configuration.GetConnectionString("IdentityConnection")
    ?? throw new InvalidOperationException("IdentityConnection string not found.");

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseMySql(identityConnectionString, ServerVersion.AutoDetect(identityConnectionString)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // options.SignIn.RequireConfirmedAccount = true; // Email confirmation required
})
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI(); // Optional: Adds default Identity UI for Razor Pages

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.Cookie.MaxAge = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true; // Optional: Enhance security
    options.Cookie.SameSite = SameSiteMode.Lax; // Ensure compatibility
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensure HTTPS
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add Universal Report services
builder.Services.AddUniversalReportServices();

var app = builder.Build();

try
{
    if (false)
    {
        string regularUserId = null;

        // Seed IdentityDbContext (Admin and Regular User)
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Starting IdentityDbContext seeding...");

            // Seed Admin User
            await IdentityDbInitializer.SeedAdminUser(
                services,
                adminEmail: "admin@example.com",
                adminPassword: "Admin@123!");

            // Seed Regular User and capture UserId
            regularUserId = await IdentityDbInitializer.SeedRegularUser(
                services,
                userEmail: "user@example.com",
                userPassword: "User@123!");

            logger.LogInformation("Completed IdentityDbContext seeding.");
        }


        // Apply migrations for AppDbContext
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Starting AppDbContext initialization...");

            var context = services.GetRequiredService<AppDbContext>();
            await AppDbInitializer.InitializeAsync(services);

            // Explicitly dispose AppDbContext to release connections
            await context.DisposeAsync();
            logger.LogInformation("Completed AppDbContext initialization and disposed context.");
        }
 
        // Seed sample data for the regular user
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Starting UserDbInitializer...");

            if (string.IsNullOrEmpty(regularUserId))
            {
                logger.LogError("Regular user ID is null. Skipping UserDbInitializer.");
                throw new InvalidOperationException("Failed to obtain regular user ID.");
            }

            var context = services.GetRequiredService<AppDbContext>();
            await UserDbInitializer.InitializeAsync(services, regularUserId, true);
            logger.LogInformation("Completed UserDbInitializer.");

            // Explicitly dispose AppDbContext to release connections
            await context.DisposeAsync();
            logger.LogInformation("Completed AppDbContext initialization and disposed context.");

            var successLogger = app.Services.GetRequiredService<ILogger<Program>>();
            successLogger.LogInformation("Database initialization and seeding completed successfully.");
        }
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during database initialization.");
    throw;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
