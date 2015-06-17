using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;

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
            Configuration = new Configuration()
                .AddJsonFile("config.json")
                .AddJsonFile("config.dev.json", optional: false)
                .AddJsonFile("config.private.json")
                .AddEnvironmentVariables();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .ConfigureSharepoint(options =>
                {
                    options.Tenant = Configuration.Get("DocumentServices:Sharepoint:Tenant");
                    options.UserName = Configuration.Get("DocumentServices:Sharepoint:User");
                    options.Password = Configuration.Get("DocumentServices:Sharepoint:Pass");
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {
                routes.MapRoute("documents", "documents", new
                {
                    controller = "Document",
                    action = "List"
                });
                //routes.MapRoute("documents", "documents", new { controller = "Document", action = "List" });
            });
        }

        partial void ConfigurePrivate(IServiceCollection services);
    }
}
