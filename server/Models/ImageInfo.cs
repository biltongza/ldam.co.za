using System;

namespace ldam.co.za.server.Models
{
    public class ImageInfo
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DateTime CaptureDate { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string FNumber { get; set; }
        public string ExposureTime { get; set; }
        public string ShutterSpeed { get; set; }
        public string FocalLength { get; set; }
        public string ISO { get; set; }
        public string Lens { get; set; }
    }
}