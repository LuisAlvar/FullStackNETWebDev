using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using WorldCitiesAPI.Data.Models;
using WorldCitiesAPI.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace WorldCitiesAPI.Tests
{
  public static class IdentityHelper
  {

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TIdentityRole"></typeparam>
    /// <param name="roleStore"></param>
    /// <returns></returns>
    public static RoleManager<TIdentityRole> GetRoleManager<TIdentityRole>(
      IRoleStore<TIdentityRole> roleStore
    ) 
    where TIdentityRole: IdentityRole
    {
      return new RoleManager<TIdentityRole>(
        roleStore,
        new IRoleValidator<TIdentityRole>[0],
        new UpperInvariantLookupNormalizer(),
        new IdentityErrorDescriber(),
        new Mock<ILogger<RoleManager<TIdentityRole>>>().Object
      );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TIDentityUser"></typeparam>
    /// <param name="userStore"></param>
    /// <returns></returns>
    public static UserManager<TIDentityUser> GetUserManager<TIDentityUser>(
      IUserStore<TIDentityUser> userStore
    ) where TIDentityUser : IdentityUser
    {
      return new UserManager<TIDentityUser>(
        userStore,
        new Mock<IOptions<IdentityOptions>>().Object,
        new PasswordHasher<TIDentityUser>(),
        new IUserValidator<TIDentityUser>[0],
        new IPasswordValidator<TIDentityUser>[0],
        new UpperInvariantLookupNormalizer(),
        new IdentityErrorDescriber(),
        new Mock<IServiceProvider>().Object,
        new Mock<ILogger<UserManager<TIDentityUser>>>().Object
      );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TIDentityUser"></typeparam>
    /// <param name="userManager"></param>
    /// <returns></returns>
    public static SignInManager<TIDentityUser> GetSignInManager<TIDentityUser>(
      UserManager<TIDentityUser> userManager  
    ) where TIDentityUser : IdentityUser
    {
      return new SignInManager<TIDentityUser>(
          userManager,
          new Mock<IHttpContextAccessor>().Object,
          new Mock<IUserClaimsPrincipalFactory<TIDentityUser>>().Object,
          new Mock<IOptions<IdentityOptions>>().Object,
          new Mock<ILogger<SignInManager<TIDentityUser>>>().Object,
          new Mock<IAuthenticationSchemeProvider>().Object,
          new Mock<IUserConfirmation<TIDentityUser>>().Object
        );
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="webApplicationFactory"></param>
    /// <exception cref="Exception"></exception>
    public static void SeedTestUserByDIServices(CustomWebApplicationFactory<Program> webApplicationFactory)
    {
      using (var scope = webApplicationFactory.Services.CreateScope())
      {
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<ApplicationDbContext>();
        var userManager = scopedServices.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole>>();

        // Ensure roles exist
        var roles = new[] { "RegisteredUser", "Administrator" };
        foreach (var role in roles)
        {
          if (!roleManager.RoleExistsAsync(role).Result)
          {
            roleManager.CreateAsync(new IdentityRole(role)).Wait();
          }
        }

        // Create a test user
        var testUser = new ApplicationUser
        {
          UserName = "DarkLord",
          Email = "DarkLord@email.com",
          EmailConfirmed = true
        };

        if (userManager.FindByNameAsync(testUser.UserName).Result == null)
        {
          var result = userManager.CreateAsync(testUser, "12345@Tech$").Result;
          if (result.Succeeded)
          {
            userManager.AddToRoleAsync(testUser, "RegisteredUser").Wait();
          }
          else
          {
            throw new Exception("Failed to create test user: " + string.Join(", ", result.Errors));
          }
        }
      }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static string ExpireGivenToken(string token)
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var jwtToken = tokenHandler.ReadJwtToken(token);
      // Create a new token with the same claims but expired
      var expiredToken = new JwtSecurityToken(
          issuer: jwtToken.Issuer,
          audience: jwtToken.Audiences.First(),
          claims: jwtToken.Claims,
          notBefore: DateTime.UtcNow.AddHours(-2), // Set Not Before to 2 hours in the past
          expires: DateTime.UtcNow.AddHours(-1),    // Set Expiration to 1 hour in the past
          signingCredentials: new SigningCredentials(
              new SymmetricSecurityKey(Encoding.UTF8.GetBytes("8966efa9-1a25-48fe-9966-d76040bacd85")),
              SecurityAlgorithms.HmacSha256));

      var expiredTokenString = tokenHandler.WriteToken(expiredToken);
      return expiredTokenString;
    }

  }
}
