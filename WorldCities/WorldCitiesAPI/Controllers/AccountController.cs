using System.IdentityModel.Tokens.Jwt;
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
    private readonly JwtHandler _jwtHandler;

    public AccountController(
      ApplicationDbContext context,
      UserManager<ApplicationUser> userManager,
      JwtHandler jwtHandler) 
    {
      _context = context;
      _userManager = userManager;
      _jwtHandler = jwtHandler;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
      var user = await _userManager.FindByNameAsync(loginRequest.Email);
      if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password)) 
      {
        return Unauthorized(new LoginResult()
        {
          Success = false,
          Message = "Invalid Email or Password"
        });
      }

      var secToken = await _jwtHandler.GetTokenAsync(user);
      var jwt = new JwtSecurityTokenHandler().WriteToken(secToken);
      return Ok(new LoginResult()
      {
        Success = true,
        Message = "Login successful",
        Token = jwt
      });

    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterRequest registerRequest)
    {
      // Checking if user via email exists
      if (await _userManager.FindByEmailAsync(registerRequest.Email) != null)
      {
        return BadRequest(new RegisterResult()
        {
          Success = false,
          Message = "Email already taken"
        });
      }

      // Checking if user via Username exists
      if (await _userManager.FindByNameAsync(registerRequest.UserName) != null)
      {
        return BadRequest(new RegisterResult()
        {
          Success = false,
          Message = "Username already taken"
        });
      }

      // Create a new standard ApplicaitonUser account
      ApplicationUser newUser = new ApplicationUser()
      {
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

      return Ok(new RegisterResult()
      {
        Success = true,
        Message = "Registered User"
      });
    }

  }
}
