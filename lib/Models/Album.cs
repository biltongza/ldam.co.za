using System.Collections.Generic;

namespace ldam.co.za.lib.Models
{
    public class Album
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public IDictionary<string, ImageMetadata> Images { get; set; } = new Dictionary<string, ImageMetadata>();
    }
}