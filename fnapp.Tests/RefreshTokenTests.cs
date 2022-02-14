
using System;
using System.Threading.Tasks;
using ldam.co.za.fnapp.Services;
using ldam.co.za.lib.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ldam.co.za.fnapp.Tests;

public class RefreshTokenTests
{
    private readonly Mock<IOptionsSnapshot<FunctionAppLightroomOptions>> mockOptions = new();
    private readonly Mock<ILogger<RefreshTokenService>> mockLogger = new();
    private readonly Mock<ISecretService> mockSecretService = new();
    private readonly Mock<IClock> mockClock = new();
    private readonly Mock<ILightroomTokenService> mockLightroomTokenService = new();
    private readonly RefreshTokenService refreshTokenService;

    const int refreshWindowMinutes = 5;
    const string testAccessToken = "eyJhbGciOiJIUzI1NiJ9.eyJjcmVhdGVkX2F0IjoiMTYyODkyOTQxNDg1NSIsInR5cGUiOiJhY2Nlc3NfdG9rZW4iLCJleHBpcmVzX2luIjoiODY0MDAwMDAifQ.WuBdsVfyidoOc-OArRAAoawXbfvQ-MAtpHZ78IaLk7Q";
    const string testRefreshToken = "eyJhbGciOiJIUzI1NiJ9.eyJjcmVhdGVkX2F0IjoiMTYyODkyOTQxNDg1NSIsInR5cGUiOiJyZWZyZXNoX3Rva2VuIiwiZXhwaXJlc19pbiI6IjEyMDk2MDAwMDAifQ.VRzzarQIKvocvckUBIbrGSDZMUyanhEjdFdEwh8v2mQ";

    public RefreshTokenTests()
    {
        mockOptions.SetupGet(x => x.Value).Returns(new FunctionAppLightroomOptions
        {
            RefreshTokenWindowMinutes = refreshWindowMinutes.ToString()
        });
        mockSecretService.Setup(x => x.GetSecret(lib.Constants.KeyVault.LightroomAccessToken)).Returns(Task.FromResult(testAccessToken));
        mockSecretService.Setup(x => x.GetSecret(lib.Constants.KeyVault.LightroomRefreshToken)).Returns(Task.FromResult(testRefreshToken));
        refreshTokenService = new RefreshTokenService(
            mockSecretService.Object,
            mockClock.Object,
            mockOptions.Object,
            mockLogger.Object,
            mockLightroomTokenService.Object
        );
    }

    [Fact]
    public async Task ShouldBailIfRefreshTokenExpired()
    {
        mockClock.Setup(x => x.Now()).Returns(new DateTime(2022, 8, 14, 10, 30, 00, DateTimeKind.Local));
        await Assert.ThrowsAsync<InvalidOperationException>(() => refreshTokenService.RefreshAccessToken());
    }

    [Fact]
    public async Task ShouldNotRefreshAccessTokenOutsideOfWindow()
    {
        mockClock.Setup(x => x.Now()).Returns(new DateTime(2021, 8, 14, 10, 30, 00, DateTimeKind.Local));
        await refreshTokenService.RefreshAccessToken();
        mockSecretService.Verify(x => x.SetSecret(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ShouldRefreshTokenWithinWindow()
    {
        mockClock.Setup(x => x.Now()).Returns(new DateTime(2021, 8, 16, 10, 30, 00, DateTimeKind.Local));
        await refreshTokenService.RefreshAccessToken();
        mockLightroomTokenService.Verify(x => x.UpdateAccessToken(), Times.Once);
    }
}
