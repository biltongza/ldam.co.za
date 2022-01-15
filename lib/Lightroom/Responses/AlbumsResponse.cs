namespace ldam.co.za.lib.Lightroom;

public class AlbumsResponse
{
    public string Base { get; set; }
    public IEnumerable<AlbumResponse> Resources { get; set; }
    public IDictionary<string, Link> Links { get; set; }
}
