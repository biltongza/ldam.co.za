using System;

namespace ldam.co.za.server.Clients.Lightroom
{
    public class CatalogResponse : BaseResponse
    {
        public CatalogPayload Payload { get; set; }
    }
}