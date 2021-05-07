using System;
using System.Threading.Tasks;
using ldam.co.za.server.Clients.Lightroom;
using Microsoft.AspNetCore.Authentication;
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
        private ILightroomClient lightroomClient;
        public AuthController(ILightroomClient lightroomClient)
        {
            this.lightroomClient = lightroomClient;
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

        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> UserDetails()
        {
            var catalog = await lightroomClient.GetCatalog();
            return Json(catalog);
        }
    }
}