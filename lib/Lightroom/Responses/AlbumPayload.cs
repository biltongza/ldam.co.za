namespace ldam.co.za.lib.Lightroom;

public class AlbumPayload : BasePayload
{
    public required string Name { get; set; }
    public AlbumParent? Parent { get; set; }
}

public class AlbumParent
{
    public required string Id { get; set; }
}
