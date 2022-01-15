namespace ldam.co.za.lib.Lightroom;

public class Settings
{
    public IDictionary<string, object> Universal { get; set; }
    public IDictionary<string, string> Desktop { get; set; }
    public IDictionary<string, string> Web { get; set; }
    public IDictionary<string, string> Mobile { get; set; }
    public IDictionary<string, string> Photosdk { get; set; }
}
