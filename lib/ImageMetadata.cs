namespace ldam.co.za.lib;
public class ImageMetadata
{
    public required string Id { get; set; }
    public required IDictionary<string, string> Hrefs { get; set; }
    public required DateTime CaptureDate { get; set; }
    public string? CameraModel { get; set; }
    public string? FNumber { get; set; }
    public string? ShutterSpeed { get; set; }
    public string? FocalLength { get; set; }
    public string? ISO { get; set; }
    public string? Lens { get; set; }
    public string? Title { get; set; }
    public string? Caption { get; set; }
    public required DateTime LastModified { get; set; }
    public required string AspectRatio { get; set; }
}