namespace ldam.co.za.lib.Lightroom;

public class Asset
{
    public string Id { get; set; }
    public string Type { get; set; }
    public string Subtype { get; set; }
    public string Updated { get; set; }
    public string Created { get; set; }
    public IDictionary<string, Link> Links { get; set; }
    public AssetPayload Payload { get; set; }
}
