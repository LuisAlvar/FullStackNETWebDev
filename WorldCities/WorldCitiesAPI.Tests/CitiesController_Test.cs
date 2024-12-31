using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCitiesAPI.Controllers;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI.Tests
{
  public class CitiesController_Test
  {
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetCity()
    {
      /// Arrange
      /// todo: define the required assets
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: "WorldCities")
        .Options;
      using var context = new ApplicationDbContext(options);
      context.Add(new City()
      {
        Id = 1,
        CountryId = 1,
        Lat = 1,
        Lon = 1,
        Name = "TestCity1"
      });
      context.SaveChanges();

      var mockILogger = new Mock<ILogger<CitiesController>>();
      ILogger<CitiesController> logger = mockILogger.Object;

      var controller = new CitiesController(logger, context);
      City? city_existing = null;
      City? city_notExisting = null;

      /// Act 
      city_existing = (await controller.GetCity(1)).Value;
      city_notExisting = (await controller.GetCity(2)).Value;

      /// Assert
      Assert.NotNull(city_existing);
      Assert.Null(city_notExisting);
    }
  }
}
