using Microsoft.AspNetCore.Identity;

namespace WorldCitiesAPI.Data.Models
{
  /// <summary>
  /// Notes:
  /// <br/>
  /// We dont neet to implement anything here; we'll just extend the IdentityUser base class, which already
  /// contains everything we need for now. 
  /// </summary>
  public class ApplicationUser : IdentityUser
  {

  }
}
