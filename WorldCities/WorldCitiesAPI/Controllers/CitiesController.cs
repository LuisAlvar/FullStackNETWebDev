using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CitiesController : ControllerBase
{
  private readonly ILogger<CitiesController> _logger;
  private readonly ApplicationDbContext _context;

  public CitiesController(ILogger<CitiesController> logger, ApplicationDbContext context)
  {
    _logger = logger;
    _context = context;
  }

  /// <summary>
  /// To get all of the cities
  /// </summary>
  /// <returns>Return a list of cities</returns>
  [HttpGet]
  public async Task<ICollection<City>> GetCities()
  {
    return await _context.Cities.AsNoTracking().ToListAsync();
  }

  /// <summary>
  /// To fetch a particular city record
  /// </summary>
  /// <param name="id">A city identifier</param>
  /// <returns>Return a city object</returns>
  [HttpGet("{id}")]
  public async Task<ActionResult<City>> GetCity(int id)
  {
    var city = await _context.Cities.FindAsync(id);
    if (city == null) return NotFound();
    return city;
  }

  /// <summary>
  /// To update a particular city record
  /// </summary>
  /// <param name="id">A city record identifier</param>
  /// <param name="city">Update city  record data</param>
  /// <returns>Return details on how to fetch the new city record via web service</returns>
  [HttpPut("{id}")]
  public async Task<IActionResult> PutCity(int id, City city)
  {
    if (id != city.Id) return BadRequest();
    _context.Entry(city).State = EntityState.Modified;

    try
    {
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
      if (!_context.Cities.Any(c => c.Id == id)) return NotFound(nameof(City));
      throw;
    }
    return CreatedAtAction("GetCity", new { id = city.Id }, city);
  }

  /// <summary>
  /// To add a particular new city record
  /// </summary>
  /// <param name="city">Possible new city record data</param>
  /// <returns>Return details on how to fetch the new city record via web service</returns>
  [HttpPost]
  public async Task<ActionResult<City>> PostCity(City city)
  {
    var possibleCity = _context.Cities
      .AsNoTracking()
      .Where(c => c.Name == city.Name && c.Lon == city.Lon && c.Lat == city.Lat).ToList().First();
    if (possibleCity == null)
    {
      _context.Cities.Add(city);
      await _context.SaveChangesAsync();
    }

    return CreatedAtAction("GetCity", new { id = city.Id }, city);
  }

  /// <summary>
  /// To delete a paricular city entity record. 
  /// <br/>
  /// Utilization example: api/cities/5 
  /// </summary>
  /// <param name="id">A city record identifier</param>
  /// <returns>Return OK if we sucessfully delete city object. Otherwise NotFound, if such city record does not exist</returns>
  [HttpDelete("{Id}")]
  public async Task<ActionResult> DeleteCity(int id)
  {
    var city = await _context.Cities.FindAsync(id);
    if (city == null) return NotFound();
    _context.Cities.Remove(city);
    await _context.SaveChangesAsync();
    return Ok();
  }

}

