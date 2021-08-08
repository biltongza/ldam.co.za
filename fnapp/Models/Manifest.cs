using System;
using System.Collections.Generic;

namespace ldam.co.za.fnapp.Models
{
    public class Manifest
    {
        public DateTime LastModified { get; set; }
        public IDictionary<string, Album> Albums { get; set; } = new Dictionary<string, Album>();
    }
}