using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ldam.co.za
{
    [Route("{controller}")]
    public class AuthController : Controller
    {
        private readonly IConfiguration configuration;
        public AuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            var properties = new OAuthChallengeProperties()
            {
                RedirectUri = new PathString(this.configuration["AdobeAuth:CallbackPath"]),
            };

            return Challenge(properties, "adobe");
        }

        [HttpGet("callback")]
        public IActionResult Callback()
        {
            return Ok();
        }
    }
}