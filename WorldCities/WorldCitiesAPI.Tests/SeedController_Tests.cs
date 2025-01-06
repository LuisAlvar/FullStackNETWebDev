using Castle.Core.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using WorldCitiesAPI.Controllers;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.Models;
using Xunit;

namespace WorldCitiesAPI.Tests
{
  public class SeedController_Tests
  {
    /// <summary>
    /// Note:
    /// <br/>
    /// TDD approach: we create the unit test, we runt the unit test, it will obviously fail, 
    /// and then we go implement the logic behind it. Lastly, we re-run the unit test.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateDefaultUsers()
    {
      /// Arrange:
      /// Create the option instances required by the ApplicationDbContext
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: "WorldCities")
        .Options;

      // Create ILogger<SeedController>
      var mockILogger = new Mock<ILogger<SeedController>>();

      // Create a IWebHost environment mock instance
      var mockEnv = Mock.Of<IWebHostEnvironment>();

      // Create a IConfiguration mock instance
      var mockConfiguration = new Mock<IConfiguration>();
      // Means that when the indexed property of IConfiguration with the key "DefaultPasswords:RegisteredUser" is accessed, 
      // the mock should return "M0ckP$$word".
      mockConfiguration.SetupGet(x => x[It.Is<string>(s => s == "DefaultPasswords:RegisteredUser")]).Returns("M0ckP$$word");
      mockConfiguration.SetupGet(x => x[It.Is<string>(s => s == "DefaultPasswords:Administrator")]).Returns("M0ckP$$word");

      // Create a ApplicationDbContext instance using the in-memory DB
      using var context = new ApplicationDbContext(options);

      // Create a RoleManager instance
      var roleManager = IdentityHelper.GetRoleManager(new RoleStore<IdentityRole>(context));

      // Create a UserManager instance 
      var userManager = IdentityHelper.GetUserManager(new UserStore<ApplicationUser>(context));

      // Create a SeedController instance
      var controller = new SeedController(mockILogger.Object, context, mockEnv, mockConfiguration.Object, roleManager, userManager);

      // Define the variables for the users we want to test
      ApplicationUser user_Admin = null!;
      ApplicationUser user_User = null!;
      ApplicationUser user_NotExisting = null!;

      /// Act:
      /// Execute the SeedController's CreateDefaultUsers()
      /// method to create the default users (and roles)
      await controller.CreateDefaultUsers();

      // retrieve the users
      user_Admin = await userManager.FindByEmailAsync("admin@email.com");
      user_User = await userManager.FindByEmailAsync("user@email.com");
      user_NotExisting = await userManager.FindByEmailAsync("notexisting@email.com");

      /// Assert
      Assert.NotNull(user_Admin);
      Assert.NotNull(user_User);
      Assert.Null(user_NotExisting);

    }
  }
}
