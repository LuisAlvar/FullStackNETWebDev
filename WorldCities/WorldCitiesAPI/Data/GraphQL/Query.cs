using Microsoft.EntityFrameworkCore;
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortOrder"></param>
    /// <param name="filterColumn"></param>
    /// <param name="filterQuery"></param>
    /// <returns></returns>
    [Serial]
    public async Task<ApiResult<CityDTO>> GetCitiesApiResult(
      [Service] ApplicationDbContext context,
      int pageIndex = 0,
      int pageSize = 10,
      string? sortColumn = null, 
      string? sortOrder = null, 
      string? filterColumn = null, 
      string? filterQuery = null
    )
    {
      return await ApiResult<CityDTO>.CreateAsync(
        context.Cities.AsNoTracking()
        .Select(c => new CityDTO() {
          Id = c.Id,
          Name = c.Name,
          Lat = c.Lat,
          Lon = c.Lon,
          CountryId = c.Country!.Id,
          CountryName = c.Country!.Name
        }),
        pageIndex,
        pageSize,
        sortColumn,
        sortOrder,
        filterColumn,
        filterQuery
      );
    }

  }
}
