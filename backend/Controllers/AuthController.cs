using System.Net;
using System.Threading.Tasks;
using ldam.co.za.lib.Lightroom;
using ldam.co.za.lib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ldam.co.za.backend
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
                return StatusCode((int)HttpStatusCode.InternalServerError, "no access_token");
            }

            var refreshToken = await accessTokenProvider.GetRefreshToken();
            if(string.IsNullOrWhiteSpace(refreshToken))
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "no refresh_token");
            }
            
            await secretService.SetSecret(lib.Constants.KeyVault.LightroomAccessToken, accessToken);
            await secretService.SetSecret(lib.Constants.KeyVault.LightroomRefreshToken, refreshToken);
            
            return NoContent();
        }
    }
}
