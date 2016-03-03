using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc.Internal;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System;
using Microsoft.Extensions.Logging;

namespace Lsquared.DocumentServices.Host
{
    public partial class Startup
    {
        public IConfiguration Configuration
        {
            get; set;
        }

        public Startup(IHostingEnvironment env)
        {
            // Setup configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);


            if (env.IsDevelopment())
            {
                builder.AddUserSecrets("documentservices");
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc()
                .AddControllersAsServices(new Type[] { typeof(DocumentController) })
                .AddViewLocalization()
                .AddRazorOptions(o => { });

            services.ConfigureSharepoint(options =>
            {
                options.Tenant = Configuration.Get("DocumentServices:Sharepoint:Tenant", (string)null);
                options.UserName = Configuration.Get("DocumentServices:Sharepoint:User", (string)null);
                options.Password = Configuration.Get("DocumentServices:Sharepoint:Pass", (string)null);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                ////app.UseDeveloperExceptionPage();
            }
            else
            {
                ////app.UseExceptionHandler("/Home/Error");
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute("documents", "documents", new { controller = "Document", action = "List" });
            });
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
