using Imager.Server.Data;
using Imager.Server.Models.Dtos;
using Imager.Server.Models.Entities;
using Imager.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Imager.Server.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ImagerAppController : ControllerBase
{
  private readonly IWebHostEnvironment _env;
  private readonly AzureBlobStorageService _blobService;
  private readonly AppDbContext _db;

  public ImagerAppController(IWebHostEnvironment env, AzureBlobStorageService blobService, AppDbContext appDbContext)
  {
    _env = env;
    _blobService = blobService;
    _db = appDbContext;
  }

  [HttpPost("upload")]
  public async Task<IActionResult> Upload([FromForm] ImageUploadRequest request)
  {
    if (request.File == null || request.File.Length == 0) return BadRequest("No file uploaded.");

    // 1. Upload to Azure Blob (private container)
    var blobName = await _blobService.UploadAsync(request.File);

    // 2. Save metadata to SQL
    var record = new ImageRecord
    {
      Title = request.Title,
      Description = request.Description,
      BlobName = blobName,
      ContentType = request.File.ContentType,
      FileSize = request.File.Length,
      CreateAt = DateTime.UtcNow
    };

    _db.Images.Add(record);
    await _db.SaveChangesAsync();

    return Ok(new {
      id = record.Id,
      message = "Image uploaded and metadata saved"
    });
  }

  // Secure streaming endpoint (no public URL)
  [HttpGet("image/{id}")]
  public async Task<IActionResult> GetImage(string id)
  {
    var record = await _db.Images.FindAsync(id);
    if (record == null) return NotFound();

    var stream = await _blobService.GetBlobStreamAsync(record.BlobName);
    if (stream == null) return NotFound();
    return File(stream, record.ContentType);
  }

}

