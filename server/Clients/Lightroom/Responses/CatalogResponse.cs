using System;

namespace ldam.co.za.server.Clients.Lightroom
{
    public class CatalogResponse
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string Type { get; set; }
        public string Subtype { get; set; }
        public CatalogPayload Payload { get; set; }
    }
}