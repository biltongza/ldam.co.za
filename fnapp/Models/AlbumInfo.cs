public class AlbumInfo
{
    public required string Id { get; init; }
    public required string Title { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public string? ParentId { get; set; }
}