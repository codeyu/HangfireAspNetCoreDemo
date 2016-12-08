using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Hangfire;
using Hangfire.Redis;
namespace HangfireAspNetCoreDemo
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            string sConnectionString = Configuration["Data:Hangfire:ConnectionString"];
            //services.AddHangfire(x => x.UseSqlServerStorage(sConnectionString));
            services.AddHangfire(configuration => configuration.UseRedisStorage("127.0.0.1"));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Map the Dashboard to the root URL
            app.UseHangfireDashboard("");
            // Map to the '/dashboard' URL
            //app.UseHangfireDashboard("/dashboard");
            //default, Hangfire maps the dashboard to '/hangfire' URL
            //app.UseHangfireDashboard();
            app.UseHangfireServer();
            app.UseMvc();
        }
    }
}
