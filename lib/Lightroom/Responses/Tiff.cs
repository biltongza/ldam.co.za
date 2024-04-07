using System.Text.Json.Serialization;

namespace ldam.co.za.lib.Lightroom;

public class Tiff
{
    [JsonPropertyName("Make")]
    public string? Make { get; set; }
    [JsonPropertyName("Model")]
    public string? Model { get; set; }
}
