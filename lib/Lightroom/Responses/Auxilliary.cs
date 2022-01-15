using System.Text.Json.Serialization;

namespace ldam.co.za.lib.Lightroom;

public class Aux
{
    [JsonPropertyName("Lens")]
    public string Lens { get; set; }
}
