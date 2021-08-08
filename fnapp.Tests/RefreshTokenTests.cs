
using ldam.co.za.fnapp.Services;
using ldam.co.za.lib.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace ldam.co.za.fnapp.Tests
{
    public class RefreshTokenTests
    {
        private readonly Mock<IConfiguration> mockConfiguration = new();
        private readonly Mock<ILogger<RefreshTokenService>> mockLogger = new();
        private readonly Mock<ISecretService> mockSecretService = new();
        private readonly Mock<IClock> mockClock = new();
        private readonly RefreshTokenService refreshTokenService;
        
        public RefreshTokenTests()
        {
            refreshTokenService = new RefreshTokenService(
                mockSecretService.Object,
                mockClock.Object,
                mockConfiguration.Object,
                mockLogger.Object
            );
        }
    }
}