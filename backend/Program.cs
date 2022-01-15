using ldam.co.za.lib.Lightroom;
using ldam.co.za.lib.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(cfg => cfg.ForwardChallenge = "adobe")
                .AddAdobeIO("adobe", options =>
                {
                    options.ClientId = builder.Configuration[Constants.AdobeConfiguration.Auth.ClientId];
                    options.ClientSecret = builder.Configuration[Constants.AdobeConfiguration.Auth.ClientSecret];
                    options.Scope.Add("lr_partner_apis");
                    options.Scope.Add("offline_access");
                    options.SaveTokens = true;
                });
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IAccessTokenProvider, AccessTokenProvider>();
builder.Services.AddSingleton<ISecretService, SecretService>((_) => new SecretService(builder.Configuration[Constants.AzureConfiguration.KeyVaultUri]));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/auth/login", [Authorize] async (IAccessTokenProvider accessTokenProvider, ISecretService secretService) =>
{
    var accessToken = await accessTokenProvider.GetAccessToken();
    if (string.IsNullOrWhiteSpace(accessToken))
    {
        return Results.Problem("no access_token");
    }

    var refreshToken = await accessTokenProvider.GetRefreshToken();
    if (string.IsNullOrWhiteSpace(refreshToken))
    {
        return Results.Problem("no refresh_token");
    }

    await secretService.SetSecret(ldam.co.za.lib.Constants.KeyVault.LightroomAccessToken, accessToken);
    await secretService.SetSecret(ldam.co.za.lib.Constants.KeyVault.LightroomRefreshToken, refreshToken);
    return Results.Ok("Logged in!");
});

app.Run("https://localhost:5001");

