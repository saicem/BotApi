namespace BotApi
{
    using System.IO;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;

    /// <summary>
    /// start the program.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">config.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets config.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// init the services.
        /// </summary>
        /// <param name="services">the service to set.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // var connStr = Configuration.GetConnectionString("UserContext");
            // services.AddDbContext<BotContext>(opt =>
            //    opt.UseMySql(connStr, ServerVersion.AutoDetect(connStr)));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BotApi", Version = "v1" });
            });
        }

        /// <summary>
        /// configure the app.
        /// </summary>
        /// <param name="app">this app.</param>
        /// <param name="env">the env of this machine.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BotApi v1"));

            if (!Directory.Exists("./CalendarFiles"))
            {
                Directory.CreateDirectory("./CalendarFiles");
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "CalendarFiles")),
                RequestPath = "/cal",
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
