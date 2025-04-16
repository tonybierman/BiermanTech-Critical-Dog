using AutoMapper;
using BiermanTech.CriticalDog;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Reports;
using BiermanTech.CriticalDog.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
    options.AddPolicy("RequireAuthenticated", policy => policy.RequireAuthenticatedUser());
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/"); // Require auth for all pages
    options.Conventions.AllowAnonymousToPage("/Index"); // Allow anonymous for /Pages/Index
    options.Conventions.AllowAnonymousToPage("/Privacy"); // Allow anonymous for /Pages/Privacy
    options.Conventions.AllowAnonymousToFolder("/Reports"); // Allow anonymous for /Pages/Reports/*
    options.Conventions.AuthorizeFolder("/Admin", "RequireAdminRole"); // Require Admin role for /Pages/Admin/*

    // TODO: Boostratps a dmin user - must be better way
    //options.Conventions.AllowAnonymousToPage("/Admin/Users/ManageUsers"); // Allow anonymous for /Pages/Index

});

// Auto Mapper Configurations
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new DefaultMappingProfile());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Add services to the container.
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


// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Apply migrations for AppDbContext
    await AppDbInitializer.InitializeAsync(scope.ServiceProvider);

    // Apply migrations for IdentityDbContext
    await IdentityDbInitializer.SeedAdminUser(
        scope.ServiceProvider,
        adminEmail: "admin@example.com",
        adminPassword: "Admin@123!");
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
