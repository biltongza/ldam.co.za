using System;

namespace ldam.co.za.lib.Models
{
    public class ImageInfo
    {
        public string AssetId { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DateTime CaptureDate { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string FNumber { get; set; }
        public string ShutterSpeed { get; set; }
        public string FocalLength { get; set; }
        public string ISO { get; set; }
        public string Lens { get; set; }
        public string Title { get; set; }
        public string Caption { get; set; }
        public DateTime LastModified { get; set; }
    }
}