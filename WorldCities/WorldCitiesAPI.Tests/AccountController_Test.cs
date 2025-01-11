using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WorldCitiesAPI.Controllers;
using WorldCitiesAPI.Data.Models;
using WorldCitiesAPI.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace WorldCitiesAPI.Tests
{
  public class AccountController_Test
  {
    [Fact]
    public async Task RegisterUnknownUser()
    {
      // Setup the default role names
      string role_RegisteredUser = "RegisteredUser";
      string role_Administrator = "Administrator";

      /// Arrange:
      /// Create the option instances required by the ApplicationDbContext
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: "WorldCities")
        .Options;

      // Create a ApplicationDbContext instance using the in-memory DB
      using var context = new ApplicationDbContext(options);

      // Create a UserManager instance 
      var userManager = IdentityHelper.GetUserManager(new UserStore<ApplicationUser>(context));

      // Create a IConfiguration mock instance
      var mockConfiguration = new Mock<IConfiguration>();

      // Create the create jwt token handler
      var jwtHandler = new JwtHandler(mockConfiguration.Object, userManager);

      // Create a SeedController instance
      var controller = new AccountController(context, userManager, jwtHandler);

      // Create a RoleManager instance 
      var roleManager = IdentityHelper.GetRoleManager(new RoleStore<IdentityRole>(context));

      // Create the default roles (if they don't exist yet)
      if (await roleManager.FindByNameAsync(role_RegisteredUser) == null) await roleManager.CreateAsync(new IdentityRole(role_RegisteredUser));
      if (await roleManager.FindByNameAsync(role_Administrator) == null) await roleManager.CreateAsync(new IdentityRole(role_Administrator));


      // Define the variables for the users we want to test
      RegisterRequest registerRequest = new RegisterRequest();
      registerRequest.UserName = "User@gmail.com";
      registerRequest.Email = "User@email.com";
      registerRequest.Password = "Sampl3Pa$$_User";

      /// Act:
      IActionResult apiResponse = await controller.Register(registerRequest);
      RegisterResult? objResponse = null!;

      if (apiResponse is OkObjectResult okResult)
      {
        objResponse = okResult.Value as RegisterResult;
      }

      /// Assert
      Assert.NotNull(objResponse);
      Assert.True(objResponse!.Success);
      Assert.True(objResponse.Message == "Registered User");

      /// Act:
      apiResponse = await controller.Register(registerRequest);
      objResponse = null!;

      if (apiResponse is BadRequestObjectResult badRequest)
      {
        objResponse = badRequest.Value as RegisterResult;
      }

      /// Assert
      Assert.NotNull(objResponse);
      Assert.False(objResponse!.Success);
      Assert.True(objResponse.Message == "Email already taken");
    }
  }
}
