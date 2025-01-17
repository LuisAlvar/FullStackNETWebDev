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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;

namespace WorldCitiesAPI.Tests
{
  public class AccountController_Test
  {
    // Setup the default role names
    private const string role_RegisteredUser = "RegisteredUser";
    private const string role_Administrator = "Administrator";

    private readonly AccountController _accountController;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly Mock<IConfiguration> _configuration;
    private readonly JwtHandler _jwtHandler;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController_Test()
    {
      /// Arrange:
      /// Create the option instances required by the ApplicationDbContext
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
          .UseInMemoryDatabase(databaseName: "WorldCities")
          .Options;

      // Create a ApplicationDbContext instance using the in-memory DB
      _applicationDbContext = new ApplicationDbContext(options);

      // Create a UserManager instance 
      _userManager = new UserManager<ApplicationUser>(
        new UserStore<ApplicationUser>(_applicationDbContext),
        new Mock<IOptions<IdentityOptions>>().Object, 
        new PasswordHasher<ApplicationUser>(), 
        new IUserValidator<ApplicationUser>[0],
        new IPasswordValidator<ApplicationUser>[0],
        new UpperInvariantLookupNormalizer(),
        new IdentityErrorDescriber(),
        new Mock<IServiceProvider>().Object, 
        new Mock<ILogger<UserManager<ApplicationUser>>>().Object
        );

      // Create a RoleManager instance 
      _roleManager = new RoleManager<IdentityRole>(
        new RoleStore<IdentityRole>(_applicationDbContext),
        new IRoleValidator<IdentityRole>[0],
        new UpperInvariantLookupNormalizer(),
        new IdentityErrorDescriber(),
        new Mock<ILogger<RoleManager<IdentityRole>>>().Object
        );

      // Create a IConfiguration mock instance
      var _configuration = new Mock<IConfiguration>();
      _configuration.SetupGet(x => x[It.Is<string>(s => s == "JwtSettings:ExpirationTimeInMinutes")]).Returns("1");
      _configuration.SetupGet(x => x[It.Is<string>(s => s == "JwtSettings:SecurityKey")]).Returns(Guid.NewGuid().ToString());
      _configuration.SetupGet(x => x[It.Is<string>(s => s == "JwtSettings:Issuer")]).Returns("MyVeryOwnIssuer");
      _configuration.SetupGet(x => x[It.Is<string>(s => s == "JwtSettings:Audience")]).Returns("https://localhost:8080");


      // Create the create jwt token handler
      _jwtHandler = new JwtHandler(_configuration.Object, _userManager);

      // Create SignInManager instance
      _signInManager = new SignInManager<ApplicationUser>(
          _userManager,
          new Mock<IHttpContextAccessor>().Object,
          new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
          new Mock<IOptions<IdentityOptions>>().Object,
          new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
          new Mock<IAuthenticationSchemeProvider>().Object,
          new Mock<IUserConfirmation<ApplicationUser>>().Object
          );

      // Create a SeedController instance
      _accountController = new AccountController(_applicationDbContext, _userManager, _signInManager, _jwtHandler);
    }

    [Fact]
    public async Task RegisterUser()
    {
      // Create the default roles (if they don't exist yet)
      if (await _roleManager.FindByNameAsync(role_RegisteredUser) == null) await _roleManager.CreateAsync(new IdentityRole(role_RegisteredUser));
      if (await _roleManager.FindByNameAsync(role_Administrator) == null) await _roleManager.CreateAsync(new IdentityRole(role_Administrator));


      // Define the variables for the users we want to test
      RegisterRequest registerRequest = new RegisterRequest();
      registerRequest.UserName = "User@email.com";
      registerRequest.Email = "User@email.com";
      registerRequest.Password = "Sampl3Pa$$_User";

      /// Act:
      IActionResult apiResponse = await _accountController.Register(registerRequest);
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
      apiResponse = await _accountController.Register(registerRequest);
      objResponse = null!;

      if (apiResponse is BadRequestObjectResult badRequest)
      {
        objResponse = badRequest.Value as RegisterResult;
      }

      /// Assert
      Assert.NotNull(objResponse);
      Assert.False(objResponse!.Success);
      Assert.True(objResponse.Message == "Username already taken");
    }

    [Fact]
    public async Task LoginUser()
    {
      // Create the default roles (if they don't exist yet)
      if (await _roleManager.FindByNameAsync(role_RegisteredUser) == null) await _roleManager.CreateAsync(new IdentityRole(role_RegisteredUser));
      if (await _roleManager.FindByNameAsync(role_Administrator) == null) await _roleManager.CreateAsync(new IdentityRole(role_Administrator));

      // Define the variables for the users we want to test
      RegisterRequest registerRequest = new RegisterRequest();
      registerRequest.UserName = "NanoTech";
      registerRequest.Email = "NanoTech@email.com";
      registerRequest.Password = "12345@tech$";
      
      /// Act: Register
      IActionResult apiResponse = await _accountController.Register(registerRequest);
      RegisterResult? objRegisterResponse = null!;
      if (apiResponse is OkObjectResult okRegisterResult)
      {
        objRegisterResponse = okRegisterResult.Value as RegisterResult;
      }

      /// Assert: Register
      Assert.NotNull(objRegisterResponse);
      Assert.True(objRegisterResponse!.Success);
      Assert.True(objRegisterResponse.Message == "Registered User");

      // Verify: Check if user exists in the database
      var user = await _applicationDbContext.Users.SingleOrDefaultAsync(u => u.Email == registerRequest.Email);
      Assert.NotNull(user);
      Assert.Equal(registerRequest.UserName, user!.UserName);

      // Act: Login
      LoginRequest loginRequest = new LoginRequest();
      loginRequest.Email = "NanoTech@email.com";
      loginRequest.Password = "12345@tech$";
      LoginResult? objLoginResponse = null!;

      apiResponse = await _accountController.Login(loginRequest);
      if (apiResponse is OkObjectResult okLoginResult)
      {
        objLoginResponse = okLoginResult.Value as LoginResult;
      }

      /// Assert: Login 
      Assert.NotNull(objLoginResponse);
      Assert.True(objLoginResponse!.Success);
      Assert.True(!string.IsNullOrEmpty(objLoginResponse.Token));
      Assert.True(!string.IsNullOrEmpty(objLoginResponse.UserId));
    }

    [Fact]
    public async Task KeepUserLoggedIn()
    {
      // Create the default roles (if they don't exist yet)
      if (await _roleManager.FindByNameAsync(role_RegisteredUser) == null) await _roleManager.CreateAsync(new IdentityRole(role_RegisteredUser));
      if (await _roleManager.FindByNameAsync(role_Administrator) == null) await _roleManager.CreateAsync(new IdentityRole(role_Administrator));

      // Define the variables for the users we want to test
      RegisterRequest registerRequest = new RegisterRequest();
      registerRequest.UserName = "DarkLord";
      registerRequest.Email = "DarkLord@email.com";
      registerRequest.Password = "12345@tech$";

      /// Act: Register
      IActionResult apiResponse = await _accountController.Register(registerRequest);
      RegisterResult? objRegisterResponse = null!;
      if (apiResponse is OkObjectResult okRegisterResult)
      {
        objRegisterResponse = okRegisterResult.Value as RegisterResult;
      }

      /// Assert: Register
      Assert.NotNull(objRegisterResponse);
      Assert.True(objRegisterResponse!.Success);
      Assert.True(objRegisterResponse.Message == "Registered User");

      // Verify: Check if user exists in the database
      var user = await _applicationDbContext.Users.SingleOrDefaultAsync(u => u.Email == registerRequest.Email);
      Assert.NotNull(user);
      Assert.Equal(registerRequest.UserName, user!.UserName);

      // Act: Login
      LoginRequest loginRequest = new LoginRequest();
      loginRequest.Email = "DarkLord@email.com";
      loginRequest.Password = "12345@tech$";
      LoginResult? objLoginResponse = null!;

      apiResponse = await _accountController.Login(loginRequest);
      if (apiResponse is OkObjectResult okLoginResult)
      {
        objLoginResponse = okLoginResult.Value as LoginResult;
      }

      /// Assert: Login 
      Assert.NotNull(objLoginResponse);
      Assert.True(objLoginResponse!.Success);
      Assert.True(!string.IsNullOrEmpty(objLoginResponse.Token));
      Assert.True(!string.IsNullOrEmpty(objLoginResponse.UserId));
      Assert.NotNull(objLoginResponse.TokenExpiration);

      await Task.Delay((int)(objLoginResponse.TokenExpiration! * 60 * 1000));

    }



  }
}
