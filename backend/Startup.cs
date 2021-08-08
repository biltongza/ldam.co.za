using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ldam.co.za.lib.Services;
using ldam.co.za.lib.Lightroom;

namespace ldam.co.za.backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(cfg => cfg.ForwardChallenge = "adobe")
                .AddAdobeIO("adobe", options =>
                {
                    options.ClientId = Configuration[Constants.AdobeConfiguration.Auth.ClientId];
                    options.ClientSecret = Configuration[Constants.AdobeConfiguration.Auth.ClientSecret];
                    options.Scope.Add("lr_partner_apis");
                    options.Scope.Add("offline_access");
                    options.SaveTokens = true;
                });

            services.AddHttpContextAccessor();

            services.AddTransient<IAccessTokenProvider, AccessTokenProvider>();
            services.AddSingleton<ISecretService, SecretService>((_) => new SecretService(Configuration[Constants.AzureConfiguration.KeyVaultUri]));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

