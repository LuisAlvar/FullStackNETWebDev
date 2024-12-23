namespace WorldCitiesAPI.Data
{
  public class CityDTO
  {
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public double Lat {  get; set; }

    public double Lon { get; set; }

    public int CountryId { get; set; }

    public string? CountryName { get; set; } = null!;
  }
}
