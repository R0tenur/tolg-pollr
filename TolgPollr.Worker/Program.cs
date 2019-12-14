using System;
using System.Threading.Tasks;
using Coravel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TolgPollr.Worker.Invocables;
using TolgPollr.Worker.Repository;
using TolgPollr.Worker.Services;

namespace TolgPollr.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            host.Services.UseScheduler(scheduler =>
            {
                scheduler.Schedule<UpdateCheckInvocable>().EveryThirtyMinutes();
            });
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile(
                        $"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                        optional: false,
                        reloadOnChange: true
                    )
                    .AddEnvironmentVariables()
                    .Build();

                })
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddSingleton<IConfiguration>(hostingContext.Configuration);
                    services.AddTransient<UpdateCheckInvocable>();
                    services.AddScoped<ICommandExecutionService, CommandExecutionService>();
                    services.AddScoped<ICurrentStateRepository, CurrentStateRepository>();
                    services.AddScheduler();
                });
    };
}
