using System;
using System.Collections.Generic;

namespace ldam.co.za.contracts
{
    public class ImageMetadata
    {
        public string Id { get; set; }
        public IDictionary<string, string> Hrefs { get; set; } = new Dictionary<string, string>();
        public DateTime CaptureDate { get; set; }
        public string CameraMake { get; set; }
        public string CameraModel { get; set; }
        public string FNumber { get; set; }
        public string ShutterSpeed { get; set; }
        public string FocalLength { get; set; }
        public string ISO { get; set; }
        public string Lens { get; set; }
        public string Title { get; set; }
        public string Caption { get; set; }
        public DateTime LastModified { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string AspectRatio { get; set; }
    }
}