using System.Collections.Generic;

namespace ldam.co.za.server.Clients.Lightroom
{
    public class AlbumAssetResponse : BaseResponse
    {
        public string Base { get; set; }
        public IEnumerable<AssetResponse> Resources { get; set; }
    }
}