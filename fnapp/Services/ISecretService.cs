namespace ldam.co.za.fnapp.Services
{
    public interface ISecretService
    {
        string GetSecret(string key);
        void SetSecret(string key, string value);
    }
}