using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                    options.ClientId = Configuration["AdobeAuth:ClientId"];
                    options.ClientSecret = Configuration["AdobeAuth:ClientSecret"];
                    options.CallbackPath = new Microsoft.AspNetCore.Http.PathString(Configuration["AdobeAuth:CallbackPath"]);
                    options.AuthorizationEndpoint = Configuration["AdobeAuth:AuthorizationEndpoint"];
                    options.TokenEndpoint = Configuration["AdobeAuth:AuthorizationEndpoint"];
                    options.SaveTokens = false;
                    var scopes = Configuration["AdobeAuth:Scopes"].Split(',');
                    foreach(var scope in scopes)
                    {
                        options.Scope.Add(scope);
                    }
                    options.Events.OnTicketReceived += async (TicketReceivedContext context) => {

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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
