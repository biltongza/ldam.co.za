namespace ldam.co.za.lib.Lightroom;

public class AlbumPayload : BasePayload
{
    public string Name { get; set; }
    public AlbumParent Parent { get; set; }
}

public class AlbumParent
{
    public string Id { get; set; }
}
