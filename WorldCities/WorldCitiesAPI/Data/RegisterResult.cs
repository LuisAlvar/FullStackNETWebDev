﻿namespace WorldCitiesAPI.Data
{
  public class RegisterResult
  {
    /// <summary>
    /// TRUE if the login attempt is successful, FALSE otherwise.
    /// </summary>
    public bool Success { get; set; } = false;

    /// <summary>
    /// Login attempt result message
    /// </summary>
    public string Message { get; set; } = null!;

  }
}
