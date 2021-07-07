using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ldam.co.za.server.Clients.Lightroom
{
    public class Asset
    {
        public string Id { get; set; }
        public IDictionary<string, Link> Links { get; set; }
    }
}