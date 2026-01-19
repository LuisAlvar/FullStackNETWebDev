namespace Imager.Server.Models.Entities;

public class ImageRecord
{
  public int Id { get; set; }
  public string Title { get; set; }
  public string Description { get; set; }
  public string BlobName { get; set; }
  public string ContentType { get; set; }
  public long FileSize { get; set; }
  public DateTime CreateAt { get; set; }
}
