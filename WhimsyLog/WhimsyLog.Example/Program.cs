using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace WhimsyLog.Example
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                cancellationTokenSource.Cancel();
            };

            var builder = CreateHostBuilder(args);
            var host = builder.Build();

            // Run the application
            await host.RunAsync(cancellationTokenSource.Token);
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {   
                    config.SetBasePath(AppContext.BaseDirectory)
                          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true)
                          .AddEnvironmentVariables()
                          .AddCommandLine(args);
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders(); // Remove the default logger providers
                    // Read configuration from the host context
                    var configuration = hostContext.Configuration;
                    logging.AddConfiguration(configuration.GetSection("Logging")); // Load logging configuration from appsettings.json
                    logging.AddConsole(); // Add the Console logger provider
                    // Retrieve the App_Data folder path from the configuration
                    var logFolderPath = Path.Combine(AppContext.BaseDirectory, configuration["WhimsyLog:LogFolderPath"]);
                    if (!Directory.Exists(logFolderPath)) 
                    { 
                        Directory.CreateDirectory(logFolderPath);
                    }
                    //logging.AddFile(logFolderPath); // Add a custom file logger provider
                    var concurrentQueue = new ConcurrentQueue<string>();
                    logging.AddQueue(concurrentQueue);
                    logging.Services.AddHostedService<BackgroundQueueFileLogger>(x => new BackgroundQueueFileLogger(logFolderPath, concurrentQueue, configuration.GetValue<int>("WhimsyLog:BackgroundLogDelayMilliseconds")));

                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });
        }

        public class Worker : BackgroundService
        {
            private readonly ILogger<Worker> logger;

            public Worker(ILogger<Worker> logger)
            {
                this.logger = logger;
            }

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                int i = 0;
                while (!stoppingToken.IsCancellationRequested)
                {   
                    //logger.LogTrace("Trace");
                    //logger.LogDebug("Debug");
                    logger.LogInformation("Information " + i);
                    //logger.LogWarning("Warning");
                    //logger.LogError("Error");
                    //logger.LogCritical("Critical");

                    i++;
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }
}