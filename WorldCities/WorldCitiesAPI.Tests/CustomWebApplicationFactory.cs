
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using WorldCitiesAPI.Data;

namespace WorldCitiesAPI.Tests;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseEnvironment("Testing");

    builder.ConfigureAppConfiguration((context, config) =>
    {
      // Clear existing configuration providers
      config.Sources.Clear();

      // Add in-memory configuration for testing
      var inMemorySettings = new Dictionary<string, string>
            {
                { "JwtSettings:RefreshTimeInDays", "1" },
                { "JwtSettings:ExpirationTimeInMinutes", "1" },
                { "JwtSettings:SecurityKey", "8966efa9-1a25-48fe-9966-d76040bacd85" },
                { "JwtSettings:Issuer", "MyVeryOwnIssuer" },
                { "JwtSettings:Audience", "https://localhost:4200" },
                { "ConnectionStrings:DefaultConnection", "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TestDb;Integrated Security=True" }
                // Add other necessary configurations if needed
            };
      config.AddInMemoryCollection(inMemorySettings);
    });

    builder.ConfigureServices(services =>
    {
      // Remove and replace ApplicationDbContext
      services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
      services.AddDbContext<ApplicationDbContext>(options =>
      {
        options.UseInMemoryDatabase("TestDb");
      });

      // Build the service provider
      var serviceProvider = services.BuildServiceProvider();

      // Create a scope to obtain services
      using (var scope = serviceProvider.CreateScope())
      {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureCreated();
      }
    });
  }

}
