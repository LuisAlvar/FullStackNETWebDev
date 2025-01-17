namespace WorldCitiesAPI.Data
{
  public class TokenRefresh
  {
    public bool NewToken { get; set; }
    public string Message { get; set; } = null!;
    public List<string> Tokens { get; set; } = new List<string>();
  }
}
