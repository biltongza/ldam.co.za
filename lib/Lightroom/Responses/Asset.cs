namespace ldam.co.za.lib.Lightroom;

public class Asset
{
    public required string Id { get; set; }
    public required string Type { get; set; }
    public required string Subtype { get; set; }
    public required string Updated { get; set; }
    public required string Created { get; set; }
    public required IDictionary<string, Link> Links { get; set; }
    public required AssetPayload Payload { get; set; }
}
