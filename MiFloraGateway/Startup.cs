using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ElmahCore;
using ElmahCore.Mvc;
using Hangfire;
using Hangfire.Annotations;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.Dashboard;
using Hangfire.LiteDB;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiFloraGateway.Authentication;
using MiFloraGateway.Database;
using MiFloraGateway.Devices;
using MiFloraGateway.Sensors;

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
            //services.AddOData();
            var logPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "log");
            services.AddElmah<XmlFileErrorLog>(options => options.LogPath = logPath);
            //services.AddElmah();
            services.AddHttpContextAccessor();

            //Register all commands
            services.AddTransient<IDetectDeviceCommand, DetectDeviceCommand>();
            services.AddTransient<IDetectSensorCommand, DetectSensorCommand>();
            services.AddTransient<IReadBatteryAndFirmwareCommand, ReadBatteryAndFirmwareCommand>();
            services.AddTransient<IReadValuesCommand, ReadValuesCommand>();
            services.AddTransient<ISendValuesCommand, SendValuesCommand>();
            services.AddSingleton<IRunOnStartup, ReadBatteryAndFirmwareStartup>();
            services.AddSingleton<IRunOnStartup, ReadValuesSensorStartup>();

            services.AddSingleton<IDataTransmitter, DataTransmitter>();
            services.AddSingleton<IRunOnStartup>(sc => (IRunOnStartup)sc.GetRequiredService<IDataTransmitter>());

            services.AddSingleton<ISettingsManager, SettingsManager>();
            services.AddTransient<Func<Task<IdentityUser>>>(sc =>
            {
                var identity = sc.GetRequiredService<IHttpContextAccessor>().HttpContext.User;
                var userManager = sc.GetRequiredService<UserManager<IdentityUser>>();
                return () => userManager.GetUserAsync(identity);
            });

            services.AddHangfire((serviceProvider, configuration) => configuration
                .UseConsole()
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                //.UseSqlServerStorage(@"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;"));
                .UseLiteDbStorage("HangFire.db", new LiteDbStorageOptions { }));

            services.AddHangfireServer();
            services.AddHangfireConsoleExtensions();

            services.AddControllers(options =>
                {
                    options.Filters.Add<ValidatorActionFilter>();
                })
                .AddControllersAsServices()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson();
            /*.AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
                options.JsonSerializerOptions.Converters.Add(new PhysicalAddressConverter());
                options.JsonSerializerOptions.Converters.Add(new VersionConverter());
            });*/
            //services.AddDbContextPool<DatabaseContext>(builder => builder.UseSqlite("Data Source=Database.db"));
            services.AddDbContextPool<DatabaseContext>(builder => builder.UseSqlServer(@"Data Source=THOR\SQLEXPRESS;Integrated Security=True;Connect Timeout=30;Database=MiFloraGateway"));
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            }).AddEntityFrameworkStores<DatabaseContext>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                options.Secure = CookieSecurePolicy.SameAsRequest;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                 .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                 {
                     options.Cookie.Name = "MifloraAuth";
                     options.Cookie.HttpOnly = true;
                     options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                     options.Cookie.SameSite = SameSiteMode.Strict;
                     options.SlidingExpiration = true;
                     options.ExpireTimeSpan = new TimeSpan(1, 0, 0);
                 });

            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

            services.AddHttpClient<IDeviceCommunicationService, DeviceCommunicationService>();
            services.AddSingleton<IDeviceLockManager, DeviceLockManager>();
            services.AddSingleton(GraphQLSchema.MakeSchema());

            services.AddSpaStaticFiles(options =>
            {
                options.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IEnumerable<IRunOnStartup> runOnStartups, IRecurringJobManager recurringJobManager, DatabaseContext databaseContext)
        {
            databaseContext.Database.Migrate();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            app.UseElmah();
            app.UseHangfireDashboard(options: new DashboardOptions {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });

            foreach (var runOnStartup in runOnStartups)
            {
                runOnStartup.Initialize();
            }
        }
    }

    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            return httpContext.User.IsInRole(Roles.Admin);
        }
    }
}
