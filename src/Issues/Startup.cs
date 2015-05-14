using System;
using System.Linq;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Caching.Memory;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Issues.Data;

namespace Issues
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Setup configuration sources.
            var configuration = new Configuration()
                .AddJsonFile("config.json")
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This reads the configuration keys from the secret store.
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                configuration.AddUserSecrets();
            }
            configuration.AddEnvironmentVariables();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Application settings to the services container.
            services.Configure<AppSettings>(Configuration.GetSubKey("AppSettings"));

            var memoryCacheOptions = new MemoryCacheOptions
            {
                ExpirationScanFrequency = TimeSpan.FromMinutes(5)
            };
            var memoryCache = new MemoryCache(memoryCacheOptions);
            services.AddInstance(typeof(IMemoryCache), memoryCache);

            services.AddInstance(typeof(IGitHubProvider), new GitHubProvider(Configuration.Get("GitHubToken")));
            services.AddTransient<IRepositories, Repositories>();
            services.AddTransient<IMilestones, Milestones>();
            services.AddTransient<IIssues, Issues.Data.Issues>();

            // Add MVC services to the services container.
            services.AddMvc().Configure<MvcOptions>(options =>
            {
                var jsonFormatter = options.OutputFormatters
                           .Where(f => f.Instance is JsonOutputFormatter)
                           .Select(f => f.Instance as JsonOutputFormatter)
                           .FirstOrDefault();
                if (jsonFormatter != null)
                {
                    jsonFormatter
                       .SerializerSettings
                       .ContractResolver = new CamelCasePropertyNamesContractResolver();

                    jsonFormatter.SerializerSettings.TypeNameHandling = TypeNameHandling.Objects;
                }
            });;
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory)
        {
            // Configure the HTTP request pipeline.

            // Add the console logger.
            loggerfactory.AddConsole(minLevel: LogLevel.Warning);

            // Add the following to the request pipeline only in development environment.
            app.UseErrorPage(ErrorPageOptions.ShowAll);

            // Add static files to the request pipeline.
            app.UseStaticFiles();

            // Add MVC to the request pipeline.
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
