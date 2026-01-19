namespace Imager.Server.Models.Dtos;

public class ImageUploadRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public IFormFile File { get; set; }
}
