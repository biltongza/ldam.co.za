using System.Collections.Generic;

namespace ldam.co.za.server.Clients.Lightroom
{
    public class CatalogPayload
    {
        public string UserCreated { get; set; }
        public string UserUpdated { get; set; }
        public string Name { get; set; }
        public FavoritesContainer Presets { get; set; }
        public FavoritesContainer Profiles { get; set; }
        public Settings Settings { get; set; }
    }
}