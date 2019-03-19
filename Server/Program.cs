using System;
using System.Diagnostics;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }
 
        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Process Id: {Process.GetCurrentProcess().Id}");
            Console.WriteLine("Started the demo server...");
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) => logging.AddConsole())
                .UseKestrel()
                .UseStartup<Startup>();

    }
}
