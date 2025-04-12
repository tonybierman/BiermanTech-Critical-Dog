using AutoMapper;
using BiermanTech.CriticalDog;
using BiermanTech.CriticalDog.Data;
using BiermanTech.CriticalDog.Reports;
using BiermanTech.CriticalDog.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var options = new WebApplicationOptions
{
    ContentRootPath = Path.GetFullPath(Directory.GetCurrentDirectory()),
    WebRootPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
};

var builder = WebApplication.CreateBuilder(options);

// Log paths for debugging
Console.WriteLine($"ContentRootPath: {builder.Environment.ContentRootPath}");
Console.WriteLine($"WebRootPath: {builder.Environment.WebRootPath}");

builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddHttpContextAccessor();

// Authorization
builder.Services.AddAuthorization(options =>
{
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

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add Universal Report services
builder.Services.AddUniversalReportServices();

var app = builder.Build();


// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Apply migrations for AppDbContext
    var appDbContext = services.GetRequiredService<AppDbContext>();
    appDbContext.Database.Migrate();

    // Apply migrations for IdentityDbContext
    var identityDbContext = services.GetRequiredService<IdentityDbContext>();
    identityDbContext.Database.Migrate();
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
