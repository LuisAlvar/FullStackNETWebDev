using System.ComponentModel.DataAnnotations;

namespace WorldCitiesAPI.Data
{
  /// <summary>
  /// Its similar to LoginRequest but for seek of exploring: how to use username to the mix
  /// </summary>
  public class RegisterRequest
  {
    [Required(ErrorMessage = "Username is required")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = null!;
  }
}
