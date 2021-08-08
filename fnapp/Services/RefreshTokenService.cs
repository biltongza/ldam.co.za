using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using ldam.co.za.lib.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ldam.co.za.fnapp.Services
{
    public class RefreshTokenService
    {
        private readonly TimeSpan refreshWindow;
        private readonly ISecretService secretService;
        private readonly IClock clock;
        private readonly ILogger logger;
        public RefreshTokenService(
            ISecretService secretService, 
            IClock clock,
            IConfiguration configuration,
            ILogger<RefreshTokenService> logger)
        {
            this.secretService = secretService;
            this.clock = clock;
            this.logger = logger;
            refreshWindow = TimeSpan.FromMinutes(int.Parse(configuration[Constants.Configuration.Adobe.RefreshTokenWindowMinutes]));
        }

        public async Task RefreshAccessToken()
        {
            var accessToken = await secretService.GetSecret(lib.Constants.KeyVault.LightroomAccessToken);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var now = clock.Now();
            var difference = jwt.ValidTo.Subtract(now); 
            if(difference.CompareTo(refreshWindow) > 0)
            {
                // token still valid
                return;
            }

            logger.Log("hello");
        }
    }
}