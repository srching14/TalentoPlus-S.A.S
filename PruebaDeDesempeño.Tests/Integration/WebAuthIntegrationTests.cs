extern alias WebProject;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PruebaDeDesempeño.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Text.RegularExpressions;

namespace PruebaDeDesempeño.Tests.Integration;

public class WebAuthIntegrationTests : IClassFixture<WebApplicationFactory<WebProject::Program>>
{
    private readonly WebApplicationFactory<WebProject::Program> _factory;
    private readonly HttpClient _client;

    public WebAuthIntegrationTests(WebApplicationFactory<WebProject::Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var dbName = "WebAuthTestDb_" + Guid.NewGuid();
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });

                // Seed Roles
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();
                    if (!roleManager.RoleExistsAsync("Cliente").Result)
                    {
                        roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole("Cliente")).Wait();
                    }
                }
            });
        });

        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Register_ShouldCreateUserAndRedirect()
    {
        // 1. Get the Register page to get the AntiForgeryToken
        var getResponse = await _client.GetAsync("/Account/Register");
        getResponse.EnsureSuccessStatusCode();
        var html = await getResponse.Content.ReadAsStringAsync();
        
        var tokenMatch = Regex.Match(html, @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" />");
        if (!tokenMatch.Success)
        {
            // Fallback: sometimes it's inside the form differently or we might need to check if it's generated.
            // If this fails, we might need to skip CSRF check or use a different approach.
            // For now, let's assume standard MVC tag helper generation.
            throw new Exception("Anti-forgery token not found in HTML");
        }
        var token = tokenMatch.Groups[1].Value;

        // 2. Prepare Form Data
        var uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
        var email = "eliasching11@gmail.com"; // User's email for verification
        
        var formData = new Dictionary<string, string>
        {
            { "FullName", "Web User Test" },
            { "Email", email },
            { "__RequestVerificationToken", token }
        };

        // 3. Post to Register
        var postResponse = await _client.PostAsync("/Account/Register", new FormUrlEncodedContent(formData));

        // 4. Assert Redirect (Success)
        // If it fails validation, it returns 200 OK (View with errors).
        // If it succeeds, it redirects to Login.
        if (postResponse.StatusCode == HttpStatusCode.OK)
        {
             var errorHtml = await postResponse.Content.ReadAsStringAsync();
             // Try to find validation errors
             var errors = Regex.Matches(errorHtml, @"<span[^>]+class=""[^""]*field-validation-error[^""]*""[^>]*>(.*?)</span>");
             var errorList = new List<string>();
             foreach (Match match in errors)
             {
                 errorList.Add(match.Groups[1].Value);
             }
             
             var summaryErrors = Regex.Matches(errorHtml, @"<div[^>]+class=""[^""]*validation-summary-errors[^""]*""[^>]*>.*?<ul>(.*?)</ul>", RegexOptions.Singleline);
             if (summaryErrors.Count > 0)
             {
                 var lis = Regex.Matches(summaryErrors[0].Groups[1].Value, @"<li>(.*?)</li>");
                 foreach (Match match in lis)
                 {
                     errorList.Add(match.Groups[1].Value);
                 }
             }

             throw new Exception($"Registration failed with validation errors: {string.Join(", ", errorList)}. Status: {postResponse.StatusCode}.");
        }

        Assert.Equal(HttpStatusCode.Redirect, postResponse.StatusCode);
        Assert.Contains("Login", postResponse.Headers.Location?.ToString());
    }
}
