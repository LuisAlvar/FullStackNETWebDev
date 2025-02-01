using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI.Data.GraphQL
{
  public class Query
  {
    /// <summary>
    /// Get all Cities
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [Serial]
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public IQueryable<City> GetCities([Service] ApplicationDbContext context) => context.Cities;

    /// <summary>
    /// Get all Countries
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [Serial]
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Country> GetCountries([Service] ApplicationDbContext context) => context.Countries;
  }
}
