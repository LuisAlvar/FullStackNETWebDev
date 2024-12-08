using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CountriesController : ControllerBase
  {
    private readonly ILogger<CountriesController> _logger;
    private readonly ApplicationDbContext _context;

    public CountriesController(ILogger<CountriesController> logger, ApplicationDbContext context)
    {
      _logger = logger;
      _context = context;
    }

    /// <summary>
    /// To get all of the countries
    /// </summary>
    /// <returns>Return a list of countries</returns>
    [HttpGet]
    public async Task<ICollection<Country>> GetCountries()
    {
      return await _context.Countries.AsNoTracking().ToListAsync();
    }

    /// <summary>
    /// To fetch a particular country record
    /// </summary>
    /// <param name="id">A country identifier</param>
    /// <returns>Return a country object</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Country>> GetCountry(int id)
    {
      var country = await _context.Countries.FindAsync(id);
      if (country == null) return NotFound();
      return country;
    }

    /// <summary>
    /// To update a particular country record
    /// </summary>
    /// <param name="id">A country record identifier</param>
    /// <param name="country">Update country record data</param>
    /// <returns>Return details on how to fetch the new country record via web service</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCountry(int id, Country country)
    {
      if (id != country.Id) return BadRequest();
      _context.Entry(country).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!_context.Countries.Any(c => c.Id == id)) return NotFound(nameof(Country));
        throw;
      }
      return CreatedAtAction("GetCountry", new { id = country.Id }, country);
    }

    /// <summary>
    /// To add a particular new country record
    /// </summary>
    /// <param name="country">Possible new country record data</param>
    /// <returns>Return details on how to fetch the new country record via web service</returns>
    [HttpPost]
    public async Task<ActionResult<Country>> PostCity(Country country)
    {
      var possibleCountry = _context.Countries
        .AsNoTracking()
        .Where(c => c.Name == country.Name && c.ISO2 == country.ISO2 && c.ISO3 == country.ISO3).ToList().First();
      if (possibleCountry == null)
      {
        _context.Countries.Add(country);
        await _context.SaveChangesAsync();
      }

      return CreatedAtAction("GetCountry", new { id = country.Id }, country);
    }

    /// <summary>
    /// To delete a paricular country entity record. 
    /// <br/>
    /// Utilization example: api/country/5 
    /// </summary>
    /// <param name="id">A country record identifier</param>
    /// <returns>Return OK if we sucessfully delete country object. Otherwise NotFound, if such country record does not exist</returns>
    [HttpDelete("{Id}")]
    public async Task<ActionResult> DeleteCountry(int id)
    {
      var country = await _context.Countries.FindAsync(id);
      if (country == null) return NotFound();
      _context.Countries.Remove(country);
      await _context.SaveChangesAsync();
      return Ok();
    }

  }
}
