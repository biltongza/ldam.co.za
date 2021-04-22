using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
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
            services.AddTransient<CreativeCloudService>();
            services
                .AddAuthentication()
                .AddOAuth("adobe", options =>
                {
                    var handler = new HttpClientHandler();
                    handler.ServerCertificateCustomValidationCallback = (HttpRequestMessage req, X509Certificate2 cert, X509Chain chain, SslPolicyErrors errors) => {
                        Console.WriteLine("HELLO");
                        return true;
                    };
                    var httpClient = new HttpClient(handler);
                    options.ClientId = Configuration["AdobeAuth:ClientId"];
                    options.ClientSecret = Configuration["AdobeAuth:ClientSecret"];
                    options.CallbackPath = new Microsoft.AspNetCore.Http.PathString(Configuration["AdobeAuth:CallbackPath"]);
                    options.AuthorizationEndpoint = Configuration["AdobeAuth:AuthorizationEndpoint"];
                    options.TokenEndpoint = Configuration["AdobeAuth:AuthorizationEndpoint"];
                    options.Backchannel = httpClient;
                    options.SaveTokens = false;
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                    options.Scope.Clear();

                    var scopes = Configuration["AdobeAuth:Scopes"].Split(',');
                    foreach(var scope in scopes)
                    {
                        options.Scope.Add(scope);
                    }
                    options.Events.OnAccessDenied += (context) => {
                        Console.WriteLine("access denied");
                        return Task.CompletedTask;
                    };
                    options.Events.OnTicketReceived += (context) => {
                        Console.WriteLine("ticket received");
                        return Task.CompletedTask;
                    };
                    options.Events.OnCreatingTicket += (context) => {
                        Console.WriteLine("creating ticket");
                        return Task.CompletedTask;
                    };
                    options.Events.OnRedirectToAuthorizationEndpoint += (RedirectContext<OAuthOptions> context) => {
                        Console.WriteLine("redirecting to auth endpoint");
                        return Task.CompletedTask;
                    };
                    options.Events.OnRemoteFailure += (context) => {
                        Console.WriteLine("remote failure");
                        return Task.CompletedTask;
                    };
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
