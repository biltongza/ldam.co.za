using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ldam.co.za.server
{
    [Route("{controller}")]
    public class AuthController : Controller
    {
        public AuthController()
        {
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            var properties = new OAuthChallengeProperties()
            {
                RedirectUri = new PathString("/auth/user"),
            };

            return Challenge(properties, "adobe");
        }
    }
}
