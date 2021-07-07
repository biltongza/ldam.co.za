using System;
using System.Collections.Generic;

namespace ldam.co.za.server.Clients.Lightroom
{
    public abstract class BaseResponse
    {
        public string Id { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }
        public string Type { get; set; }
        public string Subtype { get; set; }
        public IDictionary<string, Link> Links { get; set; }
    }
}