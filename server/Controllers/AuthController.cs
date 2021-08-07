using System.Threading.Tasks;
using ldam.co.za.lib.Lightroom;
using ldam.co.za.lib.Services;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ldam.co.za.server
{
    [Route("{controller}")]
    public class AuthController : Controller
    {
        private IAccessTokenProvider accessTokenProvider;
        private ISecretService secretService;

        public AuthController(IAccessTokenProvider accessTokenProvider, ISecretService secretService)
        {
            this.accessTokenProvider = accessTokenProvider;
            this.secretService = secretService;
        }

        [HttpGet("login")]
        [Authorize]
        public async Task<IActionResult> Login()
        {
            var accessToken = await accessTokenProvider.GetAccessToken();
            if(string.IsNullOrWhiteSpace(accessToken))
            {
                return Forbid();
            }
            
            secretService.SetSecret(lib.Constants.KeyVault.LightroomAccessToken, accessToken);
            
            return NoContent();
        }
    }
}
