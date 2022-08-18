namespace ldam.co.za.lib;
public class Album
{
    public string Id { get; set; }
    public string Title { get; set; }
    public bool IsPortfolio { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public IDictionary<string, ImageMetadata> Images { get; set; } = new Dictionary<string, ImageMetadata>();
}
