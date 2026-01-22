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
  private readonly ILogger<ImagerAppController> _logger;
  private readonly AzureBlobStorageService _blobService;
  private readonly AppDbContext _db;

  public ImagerAppController(
    IWebHostEnvironment env,
    ILogger<ImagerAppController> logger,
    AzureBlobStorageService blobService,
    AppDbContext appDbContext)
  {
    _env = env;
    _logger = logger;
    _blobService = blobService;
    _db = appDbContext;
  }

  [HttpGet("handshake")]
  public IActionResult Handshake()
  {
    return Ok(new { status = "ok", timestamp = DateTime.UtcNow });
  }

  [HttpPost("upload")]
  public async Task<IActionResult> Upload([FromForm] ImageUploadRequest request)
  {
    _logger.LogInformation("Upload endpoint invoked...");
    if (request.File == null || request.File.Length == 0) return BadRequest("No file uploaded.");

    // 1. Upload to Azure Blob (private container)
    var blobName = await _blobService.UploadAsync(request.File);

    // 2. Save metadata to SQL
    var record = new ImageRecord
    {
      ImageId = Guid.NewGuid(),
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
      id = record.ImageId, //record.Id,
      message = "Image uploaded and metadata saved"
    });
  }

  // Secure streaming endpoint (no public URL)
  [HttpGet("image/{id}")]
  public async Task<IActionResult> GetImage(Guid id)
  {
    _logger.LogInformation("GetImage endpoint invoked...");
    var record = _db.Images.Where(img => img.ImageId == id).ToList().FirstOrDefault();
    if (record == null) return NotFound();

    var stream = await _blobService.GetBlobStreamAsync(record.BlobName);
    if (stream == null) return NotFound();
    return File(stream, record.ContentType);
  }

}

