﻿using System.IdentityModel.Tokens.Jwt;
using Azure;
using Azure.Core;
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
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
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
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
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


      var response = new LoginResult()
      {
        Success = true,
        Message = "Login successful",
      };

      var secToken = await _jwtHandler.GetAccessTokenAsync(user);
      var jwtAcc = new JwtSecurityTokenHandler().WriteToken(secToken);
      response.Tokens.Add(jwtAcc);

      var refToken = await _jwtHandler.GetRefreshTokenAsync(user);
      var jwtRef = new JwtSecurityTokenHandler().WriteToken(refToken);
      response.Tokens.Add(jwtRef);

      return Ok(response);

    }


    [HttpPost("RefreshTokens")]
    [Authorize(Roles = "RegisteredUser")]
    public async Task<IActionResult> RefreshTokens([FromBody] ActiveUser activeUser)
    {

      var user = await _userManager.FindByNameAsync(activeUser.Username);
      if (user == null) return NotFound(new TokenRefresh() { NewToken = false, Message = "Member Not Found" });

      var response = new TokenRefresh()
      {
        NewToken = true,
        Message = "New Tokens",
      };

      var secToken = await _jwtHandler.GetAccessTokenAsync(user);
      var jwtAcc = new JwtSecurityTokenHandler().WriteToken(secToken);
      response.Tokens.Add(jwtAcc); // always add the access token first 

      var refToken = await _jwtHandler.GetRefreshTokenAsync(user);
      var jwtRef = new JwtSecurityTokenHandler().WriteToken(refToken);
      response.Tokens.Add(jwtRef);

      return Ok(response);
    }

    [HttpGet("ProtectedEndpoint")]
    [Authorize]
    public IActionResult ProtectedEndpoint()
    {
      return Ok("You have accessed a protected endpoint.");
    }
  }
}
