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

// Identity DB (updated to use MySQL/MariaDB)
var identityConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__IdentityConnection")
    ?? builder.Configuration.GetConnectionString("IdentityConnection");
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseMySql(identityConnectionString, ServerVersion.AutoDetect(identityConnectionString)));
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<IdentityDbContext>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add Universal Report services
builder.Services.AddUniversalReportServices();

builder.Services.AddRazorPages();

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

app.UseAuthorization();

app.MapRazorPages();

app.Run();
