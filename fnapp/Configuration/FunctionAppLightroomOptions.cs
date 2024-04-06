public class FunctionAppLightroomOptions : ldam.co.za.lib.Configuration.LightroomOptions
{
    public required string PortfolioAlbumId { get; init; }
    public required string SizesToSync { get; init; }
    public required string RefreshTokenWindowMinutes { get; init; }
    public required string CollectionsContainerAlbumId { get; init; }
}