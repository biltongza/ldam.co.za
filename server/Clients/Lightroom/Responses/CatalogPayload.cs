using System.Collections.Generic;

namespace ldam.co.za.server.Clients.Lightroom
{
    public class CatalogPayload : BasePayload
    {
        public string Name { get; set; }
        public FavoritesContainer Presets { get; set; }
        public FavoritesContainer Profiles { get; set; }
        public Settings Settings { get; set; }
    }
}