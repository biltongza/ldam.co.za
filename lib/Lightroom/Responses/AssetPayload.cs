namespace ldam.co.za.lib.Lightroom;

public class AssetPayload
{
    public required Develop Develop { get; set; }
    public Xmp? Xmp { get; set; }
    public ImportSource? ImportSource { get; set; }
    public DateTime CaptureDate { get; set; }
}
