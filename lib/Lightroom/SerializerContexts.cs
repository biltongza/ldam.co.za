using System.Text.Json.Serialization;

namespace ldam.co.za.lib.Lightroom;

[JsonSerializable(typeof(AlbumAssetResponse))]
[JsonSerializable(typeof(AlbumsResponse))]
[JsonSerializable(typeof(AlbumEmbeddedAssetResponse))]
[JsonSerializable(typeof(CatalogResponse))]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Metadata)]
public partial class LightroomSerializerContext : JsonSerializerContext
{

}