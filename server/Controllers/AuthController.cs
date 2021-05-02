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
        public IActionResult UserDetails()
        {
            return Json(this.User);
        }
    }
}