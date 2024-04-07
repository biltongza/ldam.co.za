using System.Text.Json.Serialization;

namespace ldam.co.za.lib.Lightroom;

public class Exif
{
    [JsonPropertyName("ExposureTime")]
    public int[]? ExposureTime { get; set; }
    [JsonPropertyName("FNumber")]
    public int[]? FNumber { get; set; }
    [JsonPropertyName("FocalLength")]
    public int[]? FocalLength { get; set; }
    [JsonPropertyName("ISOSpeedRatings")]
    public int? ISOSpeedRatings { get; set; }
}
