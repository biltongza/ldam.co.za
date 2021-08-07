namespace ldam.co.za.lib.Services
{
    public interface ISecretService
    {
        string GetSecret(string key);
        void SetSecret(string key, string value);
    }
}