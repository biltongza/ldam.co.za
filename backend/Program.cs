using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ldam.co.za.backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel(options => 
                    {
                        options.ListenAnyIP(5001, opts => opts.UseHttps());
                    });
                });
    }
}
