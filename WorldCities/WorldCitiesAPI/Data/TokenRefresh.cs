namespace WorldCitiesAPI.Data
{
  public class TokenRefresh
  {
    public bool NewToken { get; set; }
    public string Message { get; set; } = null!;
    public string? Token { get; set; }
  }
}
