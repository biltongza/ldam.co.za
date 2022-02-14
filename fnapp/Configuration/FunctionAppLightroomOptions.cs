public class FunctionAppLightroomOptions : ldam.co.za.lib.Configuration.LightroomOptions
{
    public string AlbumIds { get; init; }
    public string SizesToSync { get; init; }
    public string RefreshTokenWindowMinutes { get; init; }
}