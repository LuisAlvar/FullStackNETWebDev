using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Security;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
//[Authorize(Roles = "Administrator")]
public class SeedController : ControllerBase
{
  private readonly ILogger<SeedController> _logger;
  private readonly ApplicationDbContext _context;
  private readonly IWebHostEnvironment _env;
  private readonly IConfiguration _config;
  private readonly RoleManager<IdentityRole> _roleManager;
  private readonly UserManager<ApplicationUser> _userManager;

  public SeedController(
    ILogger<SeedController> logger,
    ApplicationDbContext context,
    IWebHostEnvironment env,
    IConfiguration config,
    RoleManager<IdentityRole> roleManager,
    UserManager<ApplicationUser> userManager)
  {
    _logger = logger;
    _context = context;
    _env = env;
    _config = config;
    _roleManager = roleManager;
    _userManager = userManager;   
  }

  [HttpGet]
  public async Task<ActionResult> Import()
  {
    // prevent non-development environments from running this method
    if (!_env.IsDevelopment())
    {
      throw new SecurityException("Not allowed");
    }

    var path = System.IO.Path.Combine(_env.ContentRootPath, "Data/Source/worldcities.xlsx");
    _logger.LogInformation($"Data located at {path}");

    using var stream = System.IO.File.OpenRead(path);
    using var excelPackage = new ExcelPackage(stream);

    // get the first worksheet
    var worksheet = excelPackage.Workbook.Worksheets[0];

    // define how many rows we want to process
    var nEndRow = worksheet.Dimension.End.Row;

    // initialize the record counters
    var numberOfCountriesAdded = 0;
    var numberOfCitiesAdded = 0;

    // create a lookup dictionary
    // containing all the countries already existing 
    // into the Database (it will be empty on first run).
    var countriesByName = _context.Countries.AsNoTracking().ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

    // iterates through all rows, skipping the first one
    for (int nRow = 2; nRow <= nEndRow; nRow++)
    {
      var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];

      var countryName = row[nRow, 5].GetValue<string>();
      var iso2 = row[nRow, 6].GetValue<string>();
      var iso3 = row[nRow, 7].GetValue<string>();

      // skip this country if it already exists in the database
      if (countriesByName.ContainsKey(countryName)) continue;

      // create the Country entity and fill it with xlsx data 
      var country = new Country
      {
        Name = countryName,
        ISO2 = iso2,
        ISO3 = iso3,
      };

      // add the new country to the DB context
      await _context.Countries.AddAsync(country);

      // store the country in our lookup to retrieve its Id later on 
      countriesByName.Add(countryName, country);

      // increment the counter
      numberOfCountriesAdded++;
    }

    // save all the countries into the database
    if (numberOfCountriesAdded > 0) await _context.SaveChangesAsync();

    // create a lookup disctionary 
    // containing all the cities already existing 
    // into the Database (it will be empty on first run).
    var cities = _context.Cities.AsNoTracking().ToDictionary(x =>
      new CityKey
      {
        Name = x.Name,
        Lat = x.Lat,
        Lon = x.Lon,
        CountryId = x.CountryId,
      }
    );

    // iterates through all rows, skipping the first one
    for (int nRow = 2; nRow <= nEndRow; nRow++)
    {
      var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];

      var name = row[nRow, 1].GetValue<string>();
      var nameAscii = row[nRow, 2].GetValue<string>();
      var lat = row[nRow, 3].GetValue<double>();
      var lon = row[nRow, 4].GetValue<double>();
      var countryName = row[nRow, 5].GetValue<string>();

      // retrieve country id by countryname
      var countryId = countriesByName[countryName].Id;

      // skip this city if it already exists in the database
      if (cities.ContainsKey(
            new CityKey
            {
              Name = name,
              Lat = lat,
              Lon = lon,
              CountryId = countryId,
            })) continue;

      // create the city enity and fill it with xlsx data
      var city = new City
      {
        Name = name,
        Lat = lat,
        Lon = lon,
        CountryId = countryId
      };

      // add the new city to the DB context
      _context.Cities.Add(city);

      // increment the counter
      numberOfCitiesAdded++;
    }

    // save all the cities into the database
    if (numberOfCitiesAdded > 0) await _context.SaveChangesAsync();

    return new JsonResult(new
    {
      Cities = numberOfCitiesAdded,
      Countries = numberOfCountriesAdded
    });
  }

  [HttpGet]
  public async Task<ActionResult> CreateDefaultUsers()
  {
    // Setup the default role names
    string role_RegisteredUser = "RegisteredUser";
    string role_Administrator = "Administrator";

    // Create the default roles (if they don't exist yet)
    if (await _roleManager.FindByNameAsync(role_RegisteredUser) == null) await _roleManager.CreateAsync(new IdentityRole(role_RegisteredUser));
    if (await _roleManager.FindByNameAsync(role_Administrator) == null) await _roleManager.CreateAsync(new IdentityRole(role_Administrator));

    // Create a list to track the newly added users
    var addedUserList = new List<ApplicationUser>();

    // Check if the admin user already exists
    var email_Admin = "admin@email.com";
    if (await _userManager.FindByNameAsync(email_Admin) == null)
    {
      // Create a new admin ApplicationUser account
      var user_Admin = new ApplicationUser()
      {
        Id = Guid.NewGuid().ToString(),
        SecurityStamp = Guid.NewGuid().ToString(),
        UserName = email_Admin,
        Email = email_Admin,
      };

      // Insert the admin user into the DB
      await _userManager.CreateAsync(user_Admin, _config["DefaultPasswords:Administrator"]);

      // Assign the "RegisteredUser" and "Administrator" roles
      await _userManager.AddToRoleAsync(user_Admin, role_RegisteredUser);
      await _userManager.AddToRoleAsync(user_Admin, role_Administrator);

      // Configure the email and remove lockout
      user_Admin.EmailConfirmed = true;
      user_Admin.LockoutEnabled = false;

      // Add the admin user to the added users list
      addedUserList.Add(user_Admin);
    }

    // Check if the standard user already exists
    var email_User = "user@email.com";
    if (await _userManager.FindByNameAsync(email_User) == null)
    {
      // Create a new standard ApplicationUser account
      var user_User = new ApplicationUser()
      {
        Id = Guid.NewGuid().ToString(),
        SecurityStamp = Guid.NewGuid().ToString(),
        UserName = email_User,
        Email = email_User
      };

      // Insert the standard user into the DB
      await _userManager.CreateAsync(user_User, _config["DefaultPasswords:RegisteredUser"]);

      // Assign the "RegisteredUser" role 
      await _userManager.AddToRoleAsync(user_User, role_RegisteredUser);

      // confirm the e-mail and remove lockout
      user_User.EmailConfirmed = true;
      user_User.LockoutEnabled = false;

      // add the standard user ot the added users list
      addedUserList.Add(user_User);
    }

    // if we added at least one user, persist the changes into the DB
    if (addedUserList.Count > 0) await _context.SaveChangesAsync();

    return new JsonResult(new {
      Count = addedUserList.Count,
      Users = addedUserList
    });

  }
}

public class CityKey {
  public string Name { get; set; } = null!;
  public double Lat { get; set; }
  public double Lon { get; set; }
  public int CountryId { get; set; }
}

