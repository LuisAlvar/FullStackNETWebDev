using Microsoft.EntityFrameworkCore;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI.Data
{
  public class ApplicationDbContext: DbContext
  {
    public ApplicationDbContext(): base() {}

    public ApplicationDbContext(DbContextOptions dbContextOptions): base(dbContextOptions) {}

    public DbSet<City> Cities => Set<City>();
    public DbSet<Country> Countries => Set<Country>();
  }
}
