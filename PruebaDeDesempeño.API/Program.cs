using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PruebaDeDesempeño.Web.Data;
using PruebaDeDesempeño.Web.Models;
using PruebaDeDesempeño.Web.Services;
using OfficeOpenXml;
using QuestPDF.Infrastructure;
using dotenv.net;

// Cargar variables de entorno desde .env
DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { "../.env", ".env" }));

var builder = WebApplication.CreateBuilder(args);

// Habilitar comportamiento legacy de timestamps para Npgsql
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configurar licencias
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
QuestPDF.Settings.License = LicenseType.Community;

// Configurar DbContext (reutilizando del proyecto Web)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar Identity (requerido para UserManager)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Registrar servicios del proyecto Web
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IChatbotService, ChatbotService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Configurar JWT Authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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

builder.Services.AddAuthorization();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configurar Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Prueba de Desempeño API",
        Version = "v1",
        Description = "API REST para sistema de gestión administrativa",
        Contact = new OpenApiContact
        {
            Name = "Prueba de Desempeño",
            Email = "admin@pruebadedesempeno.com"
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

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Prueba de Desempeño API v1");
    c.RoutePrefix = string.Empty; // Swagger en la raíz: http://localhost:5001
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
