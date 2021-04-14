using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Fhi.EikUtforsker
{
    public class Program
    {
        public static IConfiguration Configuration
        {
            get
            {
                var appsettingFil = $"appsettings.{ Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json";

                var miljo = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (String.IsNullOrEmpty(miljo))
                {
                    appsettingFil = "appsettings.json";
                }

                return new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile(appsettingFil, optional: true, reloadOnChange: true)
                   .AddEnvironmentVariables()
                   .Build();

            }
        }

        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();
            try
            {
                Serilog.Debugging.SelfLog.Enable(Console.Out);
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>()
                        .UseSerilog();
                });
    }
}
