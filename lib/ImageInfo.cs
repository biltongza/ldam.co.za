namespace ldam.co.za.lib;
public class ImageInfo
{
    public required string AssetId { get; set; }
    public long FileSize { get; set; }
    public DateTime CaptureDate { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int[] FNumber { get; set; }
    public int[] ShutterSpeed { get; set; }
    public int FocalLength { get; set; }
    public int ISO { get; set; }
    public string Lens { get; set; }
    public string Title { get; set; }
    public string Caption { get; set; }
    public DateTime LastModified { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}