using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI.Data
{
  public class JwtHandler
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly double _expiresIn;
    private readonly double _refreshIn;

    public JwtHandler(IConfiguration configuration, UserManager<ApplicationUser> userManager) 
    {
      _configuration = configuration;
      _userManager = userManager;
      _expiresIn = Convert.ToDouble(_configuration["JwtSettings:ExpirationTimeInMinutes"]);
      _refreshIn = Convert.ToDouble(_configuration["JwtSettings:RefreshTimeInDays"]);
      Debug.WriteLine($"Set value for _expiresIn: {_expiresIn}");
      Debug.WriteLine($"Set value for _refreshIn: {_refreshIn}");

    }

    private SigningCredentials GetSigningCredentials()
    {
      var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecurityKey"]!);
      var secret = new SymmetricSecurityKey(key);
      return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetAccessClaimsAsync(ApplicationUser user)
    {
      var claims = new List<Claim>()
      {
        new Claim(ClaimTypes.Name, user.Email),
        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
        new Claim(JwtRegisteredClaimNames.Exp, DateTime.UtcNow.AddMinutes(_expiresIn).ToString())
      };

      foreach (var role in await _userManager.GetRolesAsync(user))
      {
        claims.Add(new Claim(ClaimTypes.Role, role));
      }
      return claims;
    }

    private async Task<List<Claim>> GetRefeshClaimsAsync(ApplicationUser user)
    {
      var claims = new List<Claim>()
      {
        new Claim(ClaimTypes.Name, user.Email),
        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
        new Claim(JwtRegisteredClaimNames.Exp, DateTime.UtcNow.AddDays(_refreshIn).ToString())
      };

      foreach (var role in await _userManager.GetRolesAsync(user))
      {
        claims.Add(new Claim(ClaimTypes.Role, role));
      }
      return claims;
    }

    public double GetRefreshTokenExpiration()
    {
      return _refreshIn;
    }

    public async Task<JwtSecurityToken> GetAccessTokenAsync(ApplicationUser user)
    {
      var jwtOptions = new JwtSecurityToken(
        issuer: _configuration["JwtSettings:Issuer"],
        audience: _configuration["JwtSettings:Audience"],
        claims: await GetAccessClaimsAsync(user),
        expires: DateTime.Now.AddMinutes(_expiresIn),
        signingCredentials: GetSigningCredentials()
        );
      return jwtOptions;
    }

    public async Task<JwtSecurityToken> GetRefreshTokenAsync(ApplicationUser user)
    {
      var jwtOptions = new JwtSecurityToken(
        issuer: _configuration["JwtSettings:Issuer"],
        audience: _configuration["JwtSettings:Audience"],
        claims: await GetRefeshClaimsAsync(user),
        expires: DateTime.Now.AddDays(_refreshIn),
        signingCredentials: GetSigningCredentials()
        );
      return jwtOptions;
    }


  }
}
