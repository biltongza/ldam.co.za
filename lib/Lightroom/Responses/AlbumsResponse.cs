namespace ldam.co.za.lib.Lightroom;

public class AlbumsResponse
{
    public required string Base { get; set; }
    public required IEnumerable<AlbumResponse> Resources { get; set; }
    public required IDictionary<string, Link> Links { get; set; }
}
