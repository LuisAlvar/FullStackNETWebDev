using Azure.Storage.Blobs;
using Microsoft.Data.SqlClient;
using Serilog;

namespace Imager.Server.Services;

/// <summary>
/// This is our Startup Health Check (One-Time Gatekeeper)
/// </summary>
public class StartupHealthCheckService
{
  private readonly IConfiguration _config;
  private readonly BlobServiceClient _blobClient;

  public StartupHealthCheckService(IConfiguration config, BlobServiceClient blobClient)
  {
    _config = config;
    _blobClient = blobClient;
  }

  /// <summary>
  /// This ensures the API never starts in a broken state.
  /// </summary>
  /// <returns></returns>
  public async Task ValidateAsync()
  {
    await CheckSqlAsync();
    await CheckBlobAsync();
  }

  private async Task CheckSqlAsync()
  {
    var connString = _config.GetConnectionString("DefaultConnection");
    try
    {
      using var conn = new SqlConnection(connString);
      await conn.OpenAsync();
      Log.Information("SQL connection successful");
    }
    catch (Exception ex)
    {
      Log.Fatal(ex, "SQL connection FAILED");
      throw;
    }
  }

  private async Task CheckBlobAsync()
  {
    try
    {
      await foreach (var container in _blobClient.GetBlobContainersAsync())
      {
        Log.Information("Blob storage reachable");
        return;
      }
    }
    catch (Exception ex)
    {
      Log.Fatal(ex, "Azure Blob Storage connection FAILED");
      throw;
    }
  }
}
