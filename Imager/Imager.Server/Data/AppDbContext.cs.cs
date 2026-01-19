using Imager.Server.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Imager.Server.Data;

public class AppDbContext: DbContext
{
  public DbSet<ImageRecord> Images => Set<ImageRecord>();

  public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
  {

  }

}
