using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using PruebaDeDesempeño.Web.Data;
using PruebaDeDesempeño.Web.Models;
using PruebaDeDesempeño.Web.Services;
using QuestPDF.Infrastructure;
using OfficeOpenXml;
using dotenv.net;

// Cargar variables de entorno desde .env
DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { "../.env", ".env" }));

var builder = WebApplication.CreateBuilder(args);

// Agregar soporte para variables de entorno
builder.Configuration.AddEnvironmentVariables();

// Habilitar comportamiento legacy de timestamps para Npgsql
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configurar licencia de EPPlus (NonCommercial)
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Configurar licencia de QuestPDF (Community)
QuestPDF.Settings.License = LicenseType.Community;

// Obtener connection string de variable de entorno o configuración
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

// Obtener JWT Key de variable de entorno o configuración
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
             ?? builder.Configuration["Jwt:Key"];

// Configurar DbContext con PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configurar Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Configuración de contraseña
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;

        // Configuración de usuario
        options.User.RequireUniqueEmail = true;

        // Configuración de bloqueo
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configurar cookies de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
});

// Registrar servicios
// Configurar HttpClient para servicios externos
builder.Services.AddHttpClient();

// Servicios personalizados
builder.Services.AddScoped<GeminiService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IExcelImportService, ExcelImportService>();
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IChatbotService, ChatbotService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

// Configurar autenticación dual: Cookies para MVC y JWT para API
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "PruebaDeDesempenoAPI",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "PruebaDeDesempenoClient",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ??
                                       "SuperSecretKeyForJWTAuthenticationPruebaDeDesempeno2024!"))
        };
    });

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TalentoPlus API",
        Version = "v1",
        Description = "API REST para Sistema de Gestión de Empleados - TalentoPlus S.A.S.",
        Contact = new OpenApiContact
        {
            Name = "TalentoPlus S.A.S.",
            Email = "admin@talentoplusadmin.com"
        }
    });

    // Configurar JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando Bearer scheme. Ejemplo: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Aplicar migraciones automáticamente y sembrar datos
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        // Sembrar usuario administrador
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await SeedDataAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al aplicar migraciones o sembrar datos.");
    }
}

// Configure the HTTP request pipeline.
// Habilitar Swagger en todos los entornos
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TalentoPlus API v1");
    c.RoutePrefix = "swagger"; // Acceder en /swagger
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Rutas para área de administración
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Método para sembrar datos iniciales
static async Task SeedDataAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
{
    // Crear roles si no existen
    string[] roles = { "Administrador", "Empleado", "Cliente" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Crear usuario administrador si no existe
    var adminEmail = "admin@talentoplusadmin.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "Administrador TalentoPlus",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Administrador");
        }
    }
}

// Hacer Program accesible para las pruebas de integración
public partial class Program
{
}

