using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Imager.Server.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
  private readonly IConfiguration _config;
  private readonly BlobServiceClient _blobClient;

  public HealthController(IConfiguration config, BlobServiceClient blobClient)
  {
    _config = config;
    _blobClient = blobClient; 
  }

  /// <summary>
  /// Is the API service alive?
  /// </summary>
  /// <returns>200 status with status of alive</returns>
  [HttpGet("live")]
  public IActionResult Live()
  {
    return Ok(new { status = "alive" });
  }

  /// <summary>
  /// Can the API service handle traffic?
  /// </summary>
  /// <returns>If ready return 200 status with ready; otherwise return 503 for SQL troubles or Azure Blob troubles.</returns>
  [HttpGet("ready")]
  public async Task<IActionResult> Ready()
  {
    // Check SQL
    try
    {
      using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
      await conn.OpenAsync();
    }
    catch
    {
      return StatusCode(503, new { status = "sql_unavailable" });
    }

    // Check Blob Storage
    try
    {
      await foreach (var _ in _blobClient.GetBlobContainersAsync())
      {
        break;
      }
    }
    catch
    {
      return StatusCode(503, new { status = "blob_unavailable" });
    }

    return Ok(new { status = "ready" });
  }
}
