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

// Lob DB
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Identity DB
var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnection") ?? throw new InvalidOperationException("Connection string 'IdentityConnection' not found.");
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(identityConnectionString));
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<IdentityDbContext>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add Universal Report services
builder.Services.AddUniversalReportServices();

builder.Services.AddRazorPages();

var app = builder.Build();

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
