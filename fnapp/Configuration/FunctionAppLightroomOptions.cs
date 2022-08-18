public class FunctionAppLightroomOptions : ldam.co.za.lib.Configuration.LightroomOptions
{
    public string PortfolioAlbumId { get; init; }
    public string SizesToSync { get; init; }
    public string RefreshTokenWindowMinutes { get; init; }
    public string CollectionsContainerAlbumId { get; init; }
}