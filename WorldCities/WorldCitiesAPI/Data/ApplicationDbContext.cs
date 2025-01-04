using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI.Data
{
  public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
  {
    public ApplicationDbContext(): base() {}

    public ApplicationDbContext(DbContextOptions dbContextOptions): base(dbContextOptions) {}

    public DbSet<City> Cities => Set<City>();
    public DbSet<Country> Countries => Set<Country>();
  }
}
