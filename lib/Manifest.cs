namespace ldam.co.za.lib;
public class Manifest
{
    public DateTime LastModified { get; set; }
    public IDictionary<string, Album> Albums { get; set; } = new Dictionary<string, Album>();
}
