using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using WorldCitiesAPI.Controllers;
using WorldCitiesAPI.Data.Models;
using WorldCitiesAPI.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using System.Text;


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
    private readonly Mock<HttpContext> _httpContextMock;
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
      _httpContextMock = new Mock<HttpContext>();
      var mockResponse = new Mock<HttpResponse>();
      var mockResponseCookies = new Mock<IResponseCookies>();
      // set up the mock repsosne to return the mock cookies
      mockResponse.Setup(r => r.Cookies).Returns(mockResponseCookies.Object);
      _httpContextMock.Setup(c => c.Response).Returns(mockResponse.Object);
      // Assign the mock HttpContext to the controller
      _accountController = new AccountController(_applicationDbContext, _userManager, _signInManager, _jwtHandler);
      _accountController.ControllerContext = new ControllerContext { HttpContext = _httpContextMock.Object };

      _accountController = new AccountController(_applicationDbContext, _userManager, _signInManager, _jwtHandler);
      _customWebApplicationFactory = factory;
      _client = _customWebApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true}); 
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
      Assert.True(objResponse.Message == "Registered Now. Please proceed to login.");

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
      Assert.True(objRegisterResponse.Message == "Registered Now. Please proceed to login.");

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
      Assert.NotEmpty(objLoginResponse!.Token);
    }

    /// <summary>
    /// This will 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AccessProtectedEndpoint_WithExpiredAccessToken_ShouldRequireRefresh_WillFail()
    {
      // Create a new JwtSecurityTokenHandler
      var tokenHandler = new JwtSecurityTokenHandler();
      // Seed the user data for DarkLord@email.com
      IdentityHelper.SeedTestUserByDIServices(_customWebApplicationFactory);

      // Arrange
      var loginModel = new { Email = "DarkLord@email.com", Password = "12345@Tech$" };
      var loginContent = new StringContent(JsonConvert.SerializeObject(loginModel), System.Text.Encoding.UTF8, "application/json");

      // Act: Login to obtain tokens
      var loginResponse = await _client.PostAsync("/api/Account/Login", loginContent);
      loginResponse.EnsureSuccessStatusCode();
      var loginResult = JsonConvert.DeserializeObject<LoginResult>(await loginResponse.Content.ReadAsStringAsync());
      Assert.True(loginResult.Success);
      Assert.NotNull(loginResult.Token);

      // Ensure the refresh token cookie is set
      Assert.True(loginResponse.Headers.TryGetValues("Set-Cookie", out var setCookieValues));
      Assert.Contains(setCookieValues, value => value.Contains("refreshToken"));

      // Simulate expired access token
      var expiredAccessToken = IdentityHelper.ExpireGivenToken(loginResult.Token);
      _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", expiredAccessToken);

      // Act: Access protected endpoint with expired token
      var protectedResponse = await _client.GetAsync("/api/Account/ProtectedEndpoint");
      Assert.Equal(HttpStatusCode.Unauthorized, protectedResponse.StatusCode);

      // Act: Refresh tokens (Note: No need to include refresh token in request body as it's in the cookie)
      ActiveUser activeUser = new ActiveUser
      {
        Email = loginModel.Email
      };

      var refreshResponse = await _client.PostAsync("/api/Account/RefreshTokens", new StringContent(JsonConvert.SerializeObject(activeUser), System.Text.Encoding.UTF8, "application/json"));
      refreshResponse.EnsureSuccessStatusCode();
      var refreshResult = JsonConvert.DeserializeObject<TokenRefresh>(await refreshResponse.Content.ReadAsStringAsync());
      Assert.True(refreshResult.NewToken);
      Assert.NotNull(refreshResult.Token);

      // Use new access token to access protected endpoint
      _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", refreshResult.Token);
      var protectedResponseAfterRefresh = await _client.GetAsync("/api/Account/ProtectedEndpoint");

      // Assert: Access protected endpoint with new token
      protectedResponseAfterRefresh.EnsureSuccessStatusCode();
      Assert.Equal(HttpStatusCode.OK, protectedResponseAfterRefresh.StatusCode);
    }

  } // END OF CLASS
}// END OF NAMESPACE
