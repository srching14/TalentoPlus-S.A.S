extern alias ApiProject;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using PruebaDeDesempeño.Web.Data;
using PruebaDeDesempeño.Web.Models;

using ApiProgram = ApiProject::Program;

namespace PruebaDeDesempeño.Tests.Integration;

/// <summary>
/// Pruebas de integración para la API de empleados
/// </summary>
public class EmployeesApiIntegrationTests : IClassFixture<WebApplicationFactory<ApiProgram>>
{
    private readonly WebApplicationFactory<ApiProgram> _factory;
    private readonly HttpClient _client;

    public EmployeesApiIntegrationTests(WebApplicationFactory<ApiProgram> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor =
                    services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IntegrationTestDb");
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                    db.Database.EnsureCreated();

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

    [Fact]
    public async Task GetDepartments_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/departments");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Tecnología", content);
    }

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
    }
}
