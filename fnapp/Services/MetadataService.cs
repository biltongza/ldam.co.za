using ldam.co.za.lib;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Metadata.Profiles.Iptc;
using ImageInfo = ldam.co.za.lib.ImageInfo;

namespace ldam.co.za.fnapp.Services;

public interface IMetadataService
{
    ImageMetadata MapAdobeMetadataToManifestMetadata(ImageInfo imageInfo);
    Task<Stream> SetImageMetadata(Stream stream, ImageInfo imageInfo);
}

public class MetadataService : IMetadataService
{
    private readonly JpegEncoder encoder;

    public MetadataService()
    {
        encoder = new JpegEncoder
        {
            Quality = 100,
        };
    }

    private const string ExifDateFormat = "yyyy:MM:dd HH:mm:ss";
    public ImageMetadata MapAdobeMetadataToManifestMetadata(ImageInfo imageInfo)
    {
        var gcd = GreatestCommonDenominator(imageInfo.Width, imageInfo.Height);

        string? exposureTime = null;
        decimal? aperture = null;
        if (imageInfo.ShutterSpeed is not null)
        {
            var exposureTimeArray = imageInfo.ShutterSpeed;
            var exposureNumerator = exposureTimeArray[0];
            var exposureDivisor = exposureTimeArray[1];
            exposureTime = exposureNumerator == 1 && exposureDivisor > 1 ? $"1/{exposureDivisor}s" : $"{exposureNumerator / exposureDivisor}s";
        }
        if (imageInfo.FNumber is not null)
        {
            var apertureArray = imageInfo.FNumber;
            aperture = decimal.Divide(apertureArray[0], apertureArray[1]);
        }

        var metadata = new ImageMetadata
        {
            CameraModel = imageInfo.Model,
            Caption = imageInfo.Caption,
            CaptureDate = imageInfo.CaptureDate,
            FNumber = $"f/{aperture}",
            FocalLength = $"{imageInfo.FocalLength}mm",
            Id = imageInfo.AssetId,
            ISO = imageInfo.ISO?.ToString(),
            LastModified = imageInfo.LastModified,
            Lens = imageInfo.Lens,
            ShutterSpeed = exposureTime,
            Title = imageInfo.Title,
            AspectRatio = $"{imageInfo.Width / gcd}:{imageInfo.Height / gcd}",
            Hrefs = new Dictionary<string, string>(),
        };

        return metadata;
    }

    public async Task<Stream> SetImageMetadata(Stream stream, ImageInfo imageInfo)
    {
        using var image = await Image.LoadAsync(stream);

        image.Metadata.ExifProfile = new ExifProfile();
        image.Metadata.ExifProfile.SetValue(ExifTag.Artist, "Logan Dam");
        image.Metadata.ExifProfile.SetValue(ExifTag.Copyright, $"This work is licensed under CC BY-NC 4.0. To view a copy of this license, visit http://creativecommons.org/licenses/by-nc/4.0/");
        if (imageInfo.FNumber is not null)
        {
            image.Metadata.ExifProfile.SetValue(ExifTag.FNumber, new Rational((uint)imageInfo.FNumber[0], (uint)imageInfo.FNumber[1]));
            image.Metadata.ExifProfile.SetValue(ExifTag.ApertureValue, new Rational((uint)imageInfo.FNumber[0], (uint)imageInfo.FNumber[1]));
        }
        if (imageInfo.ISO is not null)
            image.Metadata.ExifProfile.SetValue(ExifTag.ISOSpeedRatings, [(ushort)imageInfo.ISO]);
        if (imageInfo.ShutterSpeed is not null)
            image.Metadata.ExifProfile.SetValue(ExifTag.ExposureTime, new Rational((uint)imageInfo.ShutterSpeed[0], (uint)imageInfo.ShutterSpeed[1]));
        if (imageInfo.FocalLength is not null)
            image.Metadata.ExifProfile.SetValue(ExifTag.FocalLength, new Rational((uint)imageInfo.FocalLength, 1));
        if (imageInfo.Model is not null)
            image.Metadata.ExifProfile.SetValue(ExifTag.Model, imageInfo.Model);
        if (imageInfo.Lens is not null)
            image.Metadata.ExifProfile.SetValue(ExifTag.LensModel, imageInfo.Lens);
        image.Metadata.ExifProfile.SetValue(ExifTag.DateTimeOriginal, imageInfo.CaptureDate.ToString(ExifDateFormat));
        image.Metadata.IptcProfile = new IptcProfile();
        image.Metadata.IptcProfile.SetValue(IptcTag.CopyrightNotice, "This work is licensed under CC BY-NC 4.0. To view a copy of this license, visit http://creativecommons.org/licenses/by-nc/4.0/");
        image.Metadata.IptcProfile.SetValue(IptcTag.Credit, "Logan Dam");
        image.Metadata.IptcProfile.SetValue(IptcTag.Byline, "Logan Dam");
        image.Metadata.IptcProfile.SetValue(IptcTag.Contact, "https://twitter.com/thebiltong");
        image.Metadata.IptcProfile.SetValue(IptcTag.CreatedDate, imageInfo.CaptureDate.ToString("yyyyMMdd"));
        image.Metadata.IptcProfile.SetValue(IptcTag.CreatedTime, imageInfo.CaptureDate.ToString("HHmmsszzz").Replace(":", ""));

        var updatedStream = new MemoryStream();
        await image.SaveAsJpegAsync(updatedStream, this.encoder);
        updatedStream.Seek(0, SeekOrigin.Begin);
        return updatedStream;
    }

    static int GreatestCommonDenominator(int a, int b)
    {
        if (b == 0)
        {
            return a;
        }
        return GreatestCommonDenominator(b, a % b);
    }
}
