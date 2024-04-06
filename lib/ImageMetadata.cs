namespace ldam.co.za.lib;
public class ImageMetadata
{
    public required string Id { get; set; }
    public required IDictionary<string, string> Hrefs { get; set; }
    public required DateTime CaptureDate { get; set; }
    public required string CameraModel { get; set; }
    public required string FNumber { get; set; }
    public required string ShutterSpeed { get; set; }
    public required string FocalLength { get; set; }
    public required string ISO { get; set; }
    public required string Lens { get; set; }
    public required string Title { get; set; }
    public required string Caption { get; set; }
    public required DateTime LastModified { get; set; }
    public required string AspectRatio { get; set; }
}