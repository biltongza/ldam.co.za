using SixLabors.ImageSharp;

namespace ldam.co.za.fnapp.Services;

public interface IWebPEncoderService
{
    Task Encode(Stream source, Stream destination, CancellationToken cancellationToken = default);
}

public class WebPEncoderService : IWebPEncoderService
{
    public async Task Encode(Stream source, Stream destination, CancellationToken cancellationToken = default)
    {
        var image = await Image.LoadAsync(source, cancellationToken);
        await image.SaveAsWebpAsync(destination, cancellationToken);
    }
}