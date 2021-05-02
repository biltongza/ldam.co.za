using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Security;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ldam.co.za.server
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
                .AddCookie()
                .AddOAuth("adobe", options =>
                {
                    options.ClientId = Configuration["AdobeAuth:ClientId"];
                    options.ClientSecret = Configuration["AdobeAuth:ClientSecret"];
                    options.CallbackPath = new Microsoft.AspNetCore.Http.PathString(Configuration["AdobeAuth:CallbackPath"]);
                    options.AuthorizationEndpoint = Configuration["AdobeAuth:AuthorizationEndpoint"];
                    options.TokenEndpoint = Configuration["AdobeAuth:TokenEndpoint"];
                    options.UserInformationEndpoint = Configuration["AdobeAuth:UserInformationEndpoint"];
                    options.SaveTokens = false;
                    options.Scope.Clear();
                    options.Scope.Add(Configuration["AdobeAuth:Scopes"]);
                    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "name");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Country, "address");
                    options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
                    options.ClaimActions.MapJsonKey("urn:adobeio:accounttype", "account_type");
                    options.ClaimActions.MapJsonKey("urn:adobeio:emailverified", "email_verified");
                });
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

