namespace ldam.co.za.lib;
public class Manifest
{
    public DateTime LastModified { get; set; }
    public IList<Album> Albums { get; set; } = new List<Album>();
}
