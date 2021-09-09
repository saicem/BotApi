namespace BotApi
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    /// <summary>
    /// the main entrance.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// the main function.
        /// </summary>
        /// <param name="args">params.</param>
        public static void Main(string[] args)
        {
            InitLog();
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// create host builder.
        /// </summary>
        /// <param name="args"> args.</param>
        /// <returns> webBuilder.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void InitLog()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File($"logs/Log_.txt", rollingInterval: RollingInterval.Month)
                .CreateLogger();
        }
    }
}
