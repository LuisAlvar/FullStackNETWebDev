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
using System.IdentityModel.Tokens.Jwt;
using Azure.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using System.Text;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Microsoft.IdentityModel.Tokens;

namespace WorldCitiesAPI.Tests
{
  public class AccountController_Test: IClassFixture<CustomWebApplicationFactory<Program>>
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

    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _customWebApplicationFactory;

    public AccountController_Test(CustomWebApplicationFactory<Program> factory)
    {
      /// Arrange:
      /// Create the option instances required by the ApplicationDbContext
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
          .UseInMemoryDatabase(databaseName: "WorldCities")
          .Options;

      // Create a ApplicationDbContext instance using the in-memory DB
      _applicationDbContext = new ApplicationDbContext(options);

      // Create a UserManager instance 
      _userManager = IdentityHelper.GetUserManager<ApplicationUser>(new UserStore<ApplicationUser>(_applicationDbContext));

      // Create a RoleManager instance
      _roleManager = IdentityHelper.GetRoleManager<IdentityRole>(new RoleStore<IdentityRole>(_applicationDbContext));

      // Create SignInManager instance
      _signInManager = IdentityHelper.GetSignInManager<ApplicationUser>(_userManager);


      // Create a IConfiguration mock instance
      _configuration = new Mock<IConfiguration>();
      _configuration.SetupGet(x => x[It.Is<string>(s => s == "JwtSettings:RefreshTimeInDays")]).Returns("1");
      _configuration.SetupGet(x => x[It.Is<string>(s => s == "JwtSettings:ExpirationTimeInMinutes")]).Returns("1");
      _configuration.SetupGet(x => x[It.Is<string>(s => s == "JwtSettings:SecurityKey")]).Returns("8966efa9-1a25-48fe-9966-d76040bacd85");
      _configuration.SetupGet(x => x[It.Is<string>(s => s == "JwtSettings:Issuer")]).Returns("MyVeryOwnIssuer");
      _configuration.SetupGet(x => x[It.Is<string>(s => s == "JwtSettings:Audience")]).Returns("https://localhost:4200");
      _configuration.SetupGet(x => x[It.Is<string>(s => s == "ConnectionStrings:DefaultConnection")]).Returns("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TestDb;Integrated Security=True");

      // Create the create jwt token handler
      _jwtHandler = new JwtHandler(_configuration.Object, _userManager);

      // Create a Account instance
      _accountController = new AccountController(_applicationDbContext, _userManager, _signInManager, _jwtHandler);
      _customWebApplicationFactory = factory; 
      _client = _customWebApplicationFactory.CreateClient();  
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
      Assert.NotEmpty(objLoginResponse!.Tokens);
    }

    [Fact]
    public async Task AccessProtectedEndpoint_WithExpiredAccessToken_ShouldRequireRefresh()
    {
      // Create a new JwtSecurityTokenHandler
      var tokenHandler = new JwtSecurityTokenHandler();
      // Seed the user data for DarkLord@email.com
      IdentityHelper.SeedTestUserByDIServices(_customWebApplicationFactory);
      LoginRequest loginData = new LoginRequest
      {
        Email = "DarkLord@email.com",
        Password = "12345@Tech$"
      };

      // Act: Login -----------------------------------------------------
      LoginResult? loginResult = await _client.InvokePostAsync<LoginResult>("/api/Account/Login", JsonConvert.SerializeObject(loginData), "application/json");

      // Assert: Login 
      Assert.True(loginResult!.Success);
      Assert.Equal("Login successful", loginResult.Message);
      Assert.Equal(2, loginResult.Tokens.Count);

      // Extract the Access and Refresh Token
      string accessToken = loginResult.Tokens[0];
      string refreshToken = loginResult.Tokens[1];
      // Simualted Expired access token
      string expiredAccessToken = IdentityHelper.ExpireGivenToken(accessToken);
      _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", expiredAccessToken);
      
      // Act: Access an UnAuth Endpoint -----------------------------------
      // Attempt to access a protected endpoint
      var protectedResponse = await _client.GetAsync("/api/Account/ProtectedEndpoint");

      // Assert: Access to an UnAuth Enpoint
      Assert.Equal(HttpStatusCode.Unauthorized, protectedResponse.StatusCode);

      // Remove the expired token from headers
      _client.DefaultRequestHeaders.Authorization = null;
      // Prepare RefreshTokens request
      ActiveUser refreshTokensData = new ActiveUser
      {
        Username = "DarkLord"
      };
      _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", refreshToken);

      // Act: Refresh User Access Token -------------------------------------
      TokenRefresh? refreshResult = await _client.InvokePostAsync<TokenRefresh>("/api/Account/RefreshTokens", JsonConvert.SerializeObject(refreshTokensData), "application/json");

      // Assert that new tokens were issued
      Assert.True(refreshResult!.NewToken);
      Assert.Equal("New Tokens", refreshResult.Message);
      Assert.Equal(2, refreshResult.Tokens.Count);

      // Use the new access token to access the protected endpoint
      var newAccessToken = refreshResult.Tokens[0];
      // Set the new access token in the Authorization header
      _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", newAccessToken);

      // Act: Use new Access Token to access Auth Endpoint
      var protectedResponseAfterRefresh = await _client.GetAsync("/api/Account/ProtectedEndpoint");

      // Asert: Use new Access Token to access Auth Endpoint
      protectedResponseAfterRefresh.EnsureSuccessStatusCode();
      // Assert that the protected endpoint is accessible with the new token
      Assert.Equal(HttpStatusCode.OK, protectedResponseAfterRefresh.StatusCode);
    }

  } // END OF CLASS
}// END OF NAMESPACE
