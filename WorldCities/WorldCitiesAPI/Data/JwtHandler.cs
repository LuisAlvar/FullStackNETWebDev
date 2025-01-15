using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
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
    public readonly double _expiresIn;
    public JwtHandler(IConfiguration configuration, UserManager<ApplicationUser> userManager) 
    {
      _configuration = configuration;
      _userManager = userManager;
      _expiresIn = Convert.ToDouble(_configuration["JwtSettings:ExpirationTimeInMinutes"]);
    }

    private SigningCredentials GetSigningCredentials()
    {
      var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecurityKey"]!);
      var secret = new SymmetricSecurityKey(key);
      return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaimsAsync(ApplicationUser user)
    {
      var claims = new List<Claim>()
      {
        new Claim(ClaimTypes.Name, user.Email),
        new Claim(JwtRegisteredClaimNames.Sub, user.Email), 
        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
      };

      foreach (var role in await _userManager.GetRolesAsync(user))
      {
        claims.Add(new Claim(ClaimTypes.Role, role));
      }
      return claims;
    }

    public async Task<JwtSecurityToken> GetTokenAsync(ApplicationUser user)
    {
      var jwtOptions = new JwtSecurityToken(
        issuer: _configuration["JwtSettings:Issuer"],
        audience: _configuration["JwtSettings:Audience"],
        claims: await GetClaimsAsync(user),
        expires: DateTime.Now.AddMinutes(_expiresIn),
        signingCredentials: GetSigningCredentials()
        );
      return jwtOptions;
    }

  }
}
