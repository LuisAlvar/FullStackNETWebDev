namespace WorldCitiesAPI.Data
{
  public class LoginResult
  {

    /// <summary>
    /// TRUE if the login attempt is successful, FALSE otherwise.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Login attempt result message
    /// </summary>
    public string Message { get; set; } = null!;

    /// <summary>
    /// Expose the userId within ASP.NET Core Identity
    /// </summary>
    public string? UserId { get; set; }


    /// <summary>
    /// The JWT token if the login attempt is successful, or NULL if not
    /// </summary>
    public string? Token {  get; set; }

    /// <summary>
    /// The JWT token expiration time if the login attempt is successful, or NULL if not.
    /// <br />
    /// Time is set to minutes.
    /// </summary>
    public double? TokenExpiration { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime? TokenDTStamp { get; set; }
  }
}
