using Ecommerce.API.Extensions;
using Ecommerce.API.Filters;
using Ecommerce.API.Hubs;
using Ecommerce.Application.Helper;
using Ecommerce.Application.MappingConfig;
using Ecommerce.Application.Service;
using Ecommerce.Domain.Entity;
using Ecommerce.Domain.Repository.Interfaces;
using Ecommerce.Identity.Data;
using Ecommerce.Identity.Entities;
using Ecommerce.Identity.Service.classes;
using Ecommerce.Identity.Service.interfaces;
using Ecommerce.Identity.Utils;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Repository.classes;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Enyim.Caching;
using Enyim.Caching.Configuration;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var jwtSection = configuration.GetSection("Jwt");

// ---------- DbContexts ----------
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("IdentityConnection")));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// ---------- Identity ----------
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
    options.SignIn.RequireConfirmedEmail = true;
    options.User.RequireUniqueEmail = true;
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
    options.Lockout.MaxFailedAccessAttempts = 3;
})
.AddEntityFrameworkStores<AppIdentityDbContext>()
.AddDefaultTokenProviders();

// ---------- JWT Authentication ----------
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
var jwtKey = jwtSection.GetValue<string>("Key") ?? string.Empty;
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = true,
        ValidIssuer = jwtSection.GetValue<string>("Issuer"),
        ValidateAudience = true,
        ValidAudience = jwtSection.GetValue<string>("Audience"),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// ---------- Localization ----------
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("ar") };
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// ---------- CORS ----------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// ---------- Controllers + Filters ----------
builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>())
    .AddDataAnnotationsLocalization(options =>
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResource)));

// ---------- Swagger ----------
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---------- HttpClient, SignalR, QuestPDF, Slugify ----------
builder.Services.AddHttpClient<LocationHelper>();
builder.Services.AddFluentValidation();
builder.Services.AddSignalR();
QuestPDF.Settings.License = LicenseType.Community;
builder.Services.AddSingleton<Slugify.SlugHelper>();

// ---------- Mapster ----------
TypeAdapterConfig.GlobalSettings.Scan(typeof(ProductMappingConfig).Assembly);
TypeAdapterConfig.GlobalSettings.Scan(typeof(ProductResponseMappingConfig).Assembly);
TypeAdapterConfig.GlobalSettings.Scan(typeof(CartMappingConfig).Assembly);
TypeAdapterConfig.GlobalSettings.Scan(typeof(ReviewResponseMappingConfig).Assembly);

// ---------- Services & Repositories ----------
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<BrandService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<CheckoutService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<ProductImageService>();
builder.Services.AddScoped<FileUrlHelper>();
builder.Services.AddScoped<LocationHelper>();
builder.Services.AddScoped<CacheService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ReportingService>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IInformationUserService, InformationUserService>();
builder.Services.AddScoped<IManageUserService, ManageUserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();

// ---------- Memcached ----------
builder.Services.AddEnyimMemcached(options =>
{
    options.AddServer("51.21.192.67", 11211);
});
builder.Services.AddSingleton<IDistributedCache, MemcachedDistributedCache>();

var app = builder.Build();

// ---------- Apply Localization ----------
var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);

// ---------- Migrate DBs & Seed Identity ----------
using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    var identityDb = sp.GetRequiredService<AppIdentityDbContext>();
    identityDb.Database.Migrate();

    var appDb = sp.GetService<ApplicationDbContext>();
    appDb?.Database.Migrate();

    await IdentitySeed.SeedRolesAndAdminAsync(sp);
}

// ---------- Middleware ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseGlobalExceptionHandler();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();

// ---------- Endpoints ----------
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NotificationHub>("/hubs/notifications");
});

app.MapGet("/weatherforecast", () =>
{
    var summaries = new[]
    {
        "Freezing","Bracing","Chilly","Cool","Mild","Warm","Balmy","Hot","Sweltering","Scorching"
    };

    return Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        )).ToArray();
}).WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
