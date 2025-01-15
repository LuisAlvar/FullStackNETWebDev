using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AccountController: ControllerBase
  {
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtHandler _jwtHandler;

    public AccountController(
      ApplicationDbContext context,
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
      JwtHandler jwtHandler) 
    {
      _context = context;
      _userManager = userManager;
      _signInManager = signInManager;
      _jwtHandler = jwtHandler;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterRequest registerRequest)
    {

      // Checking if user via Username exists
      if (await _userManager.FindByNameAsync(registerRequest.UserName) != null)
      {
        return BadRequest(new RegisterResult()
        {
          Success = false,
          Message = "Username already taken"
        });
      }

      // Checking if user via email exists
      if (await _userManager.FindByEmailAsync(registerRequest.Email) != null)
      {
        return BadRequest(new RegisterResult()
        {
          Success = false,
          Message = "Email already taken"
        });
      }

      // Create a new standard ApplicaitonUser account
      ApplicationUser newUser = new ApplicationUser()
      {
        Id = Guid.NewGuid().ToString(),
        SecurityStamp = Guid.NewGuid().ToString(),
        UserName = registerRequest.UserName,
        Email = registerRequest.Email
      };

      // Insert the standard user intot the DB
      await _userManager.CreateAsync(newUser, registerRequest.Password);

      // Default role to any user is the default RegisteredUser role
      await _userManager.AddToRoleAsync(newUser, "RegisteredUser");

      // Manually confirm the email and remove lockout :: we will add 2FA 
      newUser.EmailConfirmed = true;
      newUser.LockoutEnabled = false;

      await _context.SaveChangesAsync();

      return Ok(new RegisterResult()
      {
        Success = true,
        Message = "Registered User"
      });
    }


    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
      ApplicationUser user = await _userManager.FindByEmailAsync(loginRequest.Email);
      if (user == null)
      {
        return Unauthorized(new LoginResult()
        {
          Success = false,
          Message = "Invalid Email"
        });
      }

      bool checkPassword = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
      if (!checkPassword)
      {
        return Unauthorized(new LoginResult()
        {
          Success = false,
          Message = "Invalid Password"
        });
      }

      var secToken = await _jwtHandler.GetTokenAsync(user);
      var jwt = new JwtSecurityTokenHandler().WriteToken(secToken);
      return Ok(new LoginResult()
      {
        UserId = user.Id,
        Success = true,
        Message = "Login successful",
        Token = jwt,
        TokenExpiration = _jwtHandler._expiresIn,
        TokenDTStamp = DateTime.UtcNow
      });
    }


    [HttpPost("Token")]
    [Authorize(Roles = "RegisteredUser")]
    public async Task<IActionResult> GenerateNewToken(ActiveUser activeUser)
    {
      var user = await _userManager.FindByNameAsync(activeUser.Username);
      if (user == null) return BadRequest(new TokenRefresh() { NewToken = false, Message = "Member Not Found" });
      if (user != null && user.Id != activeUser.UserId) return BadRequest(new TokenRefresh() { NewToken = false, Message = "Data does not Match" });

      var userTokens = await _userManager.GetAuthenticationTokenAsync(user!, "Default", "AccessToken");
      if (userTokens != null) return Ok(new TokenRefresh() { NewToken = false , Message = "Token is still active" });

      var secToken = await _jwtHandler.GetTokenAsync(user!);
      var jwt = new JwtSecurityTokenHandler().WriteToken(secToken);
      return Ok(new TokenRefresh()
      {
        NewToken = true,
        Message = "New Active Token",
        Token = jwt,
        TokenExpiration = _jwtHandler._expiresIn
      });
    }



  }
}
