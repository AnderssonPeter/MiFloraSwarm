using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiFlora.Common;

namespace CoreFlora
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                    .AddControllersAsServices()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                        options.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
                        options.JsonSerializerOptions.Converters.Add(new PhysicalAddressConverter());
                        options.JsonSerializerOptions.Converters.Add(new VersionConverter());
                    });
            services.AddSingleton<IHostingPort, HostingPort>();
            services.AddHostedService<DiscoverBackgroundService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostingPort hostingPort)
        {
            var port = int.Parse(app.ServerFeatures.Get<IServerAddressesFeature>().Addresses.First(x => x.StartsWith("http://", StringComparison.OrdinalIgnoreCase)).Split(":").Last());
            hostingPort.Port = port;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public interface IHostingPort
    {
        public int Port { get; set; }
    }

    public class HostingPort : IHostingPort
    {
        private int? hostingPort;
        public int Port 
        { 
            get
            {
                if (!hostingPort.HasValue)
                {
                    throw new InvalidOperationException("Failed to get hosting!");
                }
                return hostingPort.Value;
            }
            set
            {
                if (hostingPort.HasValue)
                {
                    throw new Exception("Can only set value once");
                }
                hostingPort = value;
            }
        }
    }
}
