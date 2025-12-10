extern alias ApiProject;
using ApiProgram = ApiProject::Program;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using PruebaDeDesempeño.Web.Data;
using PruebaDeDesempeño.Web.Models;

namespace PruebaDeDesempeño.Tests.Integration;

public class EmailIntegrationTests : IClassFixture<WebApplicationFactory<ApiProgram>>
{
    private readonly WebApplicationFactory<ApiProgram> _factory;
    private readonly HttpClient _client;

    public EmailIntegrationTests(WebApplicationFactory<ApiProgram> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Use InMemory DB to avoid messing with real DB, but keep real EmailService
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var dbName = "EmailTestDb_" + Guid.NewGuid();
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });
                
                // Ensure Department exists
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    db.Database.EnsureCreated();
                    if (!db.Departments.Any())
                    {
                        db.Departments.Add(new Department 
                        { 
                            Id = 1, // Force ID 1
                            Name = "Test Dept", 
                            Description = "Test", 
                            IsActive = true, 
                            CreatedAt = DateTime.UtcNow 
                        });
                        db.SaveChanges();
                    }
                }
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Register_ShouldSendEmail()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
        var registerDto = new
        {
            DocumentNumber = $"DOC{uniqueId}",
            DocumentType = "CC",
            FullName = "Elias Ching Test",
            Email = "eliasching11@gmail.com",
            DepartmentId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/employees/register", registerDto);

        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Request failed with {response.StatusCode}: {content}");
        }
        response.EnsureSuccessStatusCode();
        Assert.Contains("success", content);
        // We can't easily assert the email arrived at Mailtrap from here without their API, 
        // but a 200 OK from the controller implies SendEmailAsync executed without throwing.
    }
}
