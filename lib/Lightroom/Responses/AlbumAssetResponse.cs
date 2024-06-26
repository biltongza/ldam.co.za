namespace ldam.co.za.lib.Lightroom;

public class AlbumAssetResponse
{
    public required string Base { get; set; }
    public required IEnumerable<AlbumEmbeddedAssetResponse> Resources { get; set; }
    public IDictionary<string, Link>? Links { get; set; }
}
