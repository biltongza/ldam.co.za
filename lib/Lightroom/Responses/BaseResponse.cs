namespace ldam.co.za.lib.Lightroom;

public abstract class BaseResponse
{
    public required string Id { get; set; }
    public required string Created { get; set; }
    public required string Updated { get; set; }
    public required string Type { get; set; }
    public string? Subtype { get; set; }
    public IDictionary<string, Link>? Links { get; set; }
}
