namespace ldam.co.za.lib.Lightroom;

public interface IAccessTokenProvider
{
    Task<string?> GetAccessToken();
    Task<string?> GetRefreshToken();
}
