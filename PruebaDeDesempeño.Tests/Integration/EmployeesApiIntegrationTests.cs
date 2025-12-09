using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using PruebaDeDesempeño.Web;
using PruebaDeDesempeño.Web.Data;
using PruebaDeDesempeño.Web.Models;

namespace PruebaDeDesempeño.Tests.Integration;

/// <summary>
/// Pruebas de integración para la API de empleados
/// </summary>
public class EmployeesApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public EmployeesApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remover el DbContext existente
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Agregar DbContext usando InMemory para pruebas
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase_" + Guid.NewGuid());
                });

                // Construir el service provider
                var sp = services.BuildServiceProvider();

                // Crear scope y obtener el contexto para seed data
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                    
                    db.Database.EnsureCreated();
                    
                    // Seed data para pruebas
                    if (!db.Departments.Any())
                    {
                        db.Departments.Add(new Department
                        {
                            Id = 1,
                            Name = "Tecnología",
                            Description = "IT Department",
                            CreatedAt = DateTime.UtcNow,
                            IsActive = true
                        });
                        db.SaveChanges();
                    }
                }
            });
        });

        _client = _factory.CreateClient();
    }

    /// <summary>
    /// Prueba de integración 1: Verificar conexión a la base de datos
    /// </summary>
    [Fact]
    public async Task Database_CanConnect()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Act
        var canConnect = await context.Database.CanConnectAsync();

        // Assert
        Assert.True(canConnect, "La base de datos debe estar accesible");
    }

    /// <summary>
    /// Prueba de integración 2: Verificar que el endpoint GET /api/employees funciona
    /// </summary>
    [Fact]
    public async Task GetEmployees_ReturnsSuccessStatusCode()
    {
        // Arrange - Agregar datos de prueba
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            if (!context.Employees.Any())
            {
                context.Employees.Add(new Employee
                {
                    FullName = "Test Employee API",
                    Email = "testapi@test.com",
                    DocumentNumber = "API123456",
                    DocumentType = "CC",
                    Position = "Developer",
                    DepartmentId = 1,
                    IsActive = true,
                    Status = EmployeeStatus.Activo,
                    HireDate = DateTime.UtcNow,
                    Salary = 5000000,
                    CreatedAt = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
            }
        }

        // Act
        var response = await _client.GetAsync("/api/employees");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }

    /// <summary>
    /// Prueba de integración 3: Verificar que se pueden insertar datos en la BD
    /// </summary>
    [Fact]
    public async Task Database_CanInsertAndRetrieveEmployee()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var employee = new Employee
        {
            FullName = "Integration Test User",
            Email = "integration@test.com",
            DocumentNumber = "INT999",
            DocumentType = "CC",
            Position = "QA Engineer",
            DepartmentId = 1,
            IsActive = true,
            Status = EmployeeStatus.Activo,
            HireDate = DateTime.UtcNow,
            Salary = 4500000,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        context.Employees.Add(employee);
        await context.SaveChangesAsync();

        // Assert
        var savedEmployee = await context.Employees
            .FirstOrDefaultAsync(e => e.Email == "integration@test.com");
        
        Assert.NotNull(savedEmployee);
        Assert.Equal("Integration Test User", savedEmployee.FullName);
        Assert.Equal(4500000, savedEmployee.Salary);
    }

    /// <summary>
    /// Prueba de integración 4: Verificar que los departamentos se pueden consultar
    /// </summary>
    [Fact]
    public async Task Database_CanQueryDepartments()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Act
        var departments = await context.Departments.ToListAsync();

        // Assert
        Assert.NotEmpty(departments);
        Assert.Contains(departments, d => d.Name == "Tecnología");
    }
}
