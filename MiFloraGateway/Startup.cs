using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiFloraGateway.Database;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using MiFloraGateway.Devices;
using Microsoft.AspNet.OData.Formatter.Serialization;
using MiFloraGateway.Sensors;
using ElmahCore.Mvc;
using Hangfire.Logging;
using Hangfire;
using Hangfire.LiteDB;
using ElmahCore;
using System.IO;
using System.Reflection;
using Hangfire.Console;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet;
using System.Threading;

namespace MiFloraGateway
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
            services.AddLogging(x => x.AddHangfireConsole());
            services.AddOData();
            var logPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "log");
            services.AddElmah<XmlFileErrorLog>(options => options.LogPath = logPath);
            //services.AddElmah();
            services.AddHttpContextAccessor();

            //Register all commands
            services.AddTransient<TestCommand>();
            services.AddTransient<IDetectDeviceCommand, DetectDeviceCommand>();
            services.AddTransient<IDetectSensorCommand, DetectSensorCommand>();
            services.AddTransient<IReadBatteryAndFirmwareCommand, ReadBatteryAndFirmwareCommand>();
            services.AddTransient<IReadValuesCommand, ReadValuesCommand>();
            services.AddSingleton<IRunOnStartup, ReadBatteryAndFirmwareStartup>();
            services.AddSingleton<IRunOnStartup, ReadValuesSensorStartup>();
            services.AddSingleton<ISettingsManager, SettingsManager>();
            services.AddSingleton<IJobManager, JobManager>();

            services.AddHangfire((serviceProvider, configuration) => configuration
                .UseConsole()
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(@"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;"));
                //.UseLiteDbStorage("HangFire.db", new LiteDbStorageOptions { }));

            services.AddHangfireServer();

            services.AddMvc(options => options.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options => {
                });
            //services.AddDbContextPool<DatabaseContext>(builder => builder.UseSqlite("Data Source=Database.db"));
            services.AddDbContextPool<DatabaseContext>(builder => builder.UseSqlServer(@"Data Source=THOR\SQLEXPRESS;Integrated Security=True;Connect Timeout=30;Database=MiFloraGateway"));
            services.AddHttpClient<IDeviceService, DeviceService>();
            //services.AddTransient<IDeviceService, DeviceService>();
            //services.AddSingleton<IDeviceDetector, DeviceDetector>();
            services.AddSingleton<IDeviceLockManager, DeviceLockManager>();
            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IEnumerable<IRunOnStartup> runOnStartups, IRecurringJobManager recurringJobManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseElmah();
            app.UseHangfireDashboard();

            app.UseMvc(builder => {
                builder.Select().Expand().Filter().OrderBy().MaxTop(100).Count();
                builder.MapODataServiceRoute("odata", "odata", ODataModelBuilder.GetEdmModel());
            });

            /*app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });*/

            recurringJobManager.AddOrUpdate<TestCommand>("Test", x => x.Run(null), "0 0 0 ? * *");

            foreach (var runOnStartup in runOnStartups)
            {
                runOnStartup.Initialize();
            }
        }
    }
}
