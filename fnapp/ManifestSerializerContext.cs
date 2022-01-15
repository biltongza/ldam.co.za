using System.Text.Json.Serialization;
using ldam.co.za.lib;

[JsonSerializable(typeof(Manifest))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class ManifestSerializerContext : JsonSerializerContext
{    
}