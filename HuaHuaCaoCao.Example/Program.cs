using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HuaHuaCaoCao.Client;
using Microsoft.Extensions.DependencyInjection;

namespace HuaHuaCaoCao.Example
{
    enum Settings
    {
        Email,
        Password,
        [DefaultValue("https://eu-api.huahuacaocao.net/api/v2")]
        ApiEndpoint,
        [DefaultValue("EU")]
        Region,
        [DefaultValue("20")]
        FetchCount
    }
    class Program
    {

        static Dictionary<Settings, string> settings = new Dictionary<Settings, string>();
        static IServiceProvider serviceProvider;

        static async Task Main(string[] args)
        {
            foreach(var setting in Enum<Settings>.GetValues())
            {
                settings.Add(setting, GetSetting(setting));
            }


            var serviceCollection = new ServiceCollection()
                .AddOptions()
                .Configure<HuaHuaCaoCaoOptions>(options => {
                    options.Email = settings[Settings.Email];
                    options.Password = settings[Settings.Password];
                    options.ApiEndpoint = new Uri(settings[Settings.ApiEndpoint]);
                    options.Region = settings[Settings.Region];
                    options.FetchCount = int.Parse(settings[Settings.FetchCount]);
                })
                .AddTransient<ImageClient>()
                .AddTransient<PlantClient>();

            serviceCollection.AddHttpClient<ImageClient>();
            serviceCollection.AddHttpClient<PlantClient>();


            serviceProvider = serviceCollection.BuildServiceProvider();
            var plantClient = serviceProvider.GetService<PlantClient>();
            var imageClient = serviceProvider.GetService<ImageClient>();
            Console.WriteLine("Authenticating...");
            await plantClient.AuthenticateAsync();
            Console.WriteLine("Authenticated!");
            while (true)
            {
                Console.Write("Search: ");
                var search = Console.ReadLine();
                var results = await plantClient.SearchAsync(search);
                foreach(var result in results.Select((result, index) => new { Index = index, Item = result }))
                {
                    Console.WriteLine($"{result.Index}: {result.Item.Alias}");
                }
                Console.Write("#: ");
                var index = int.Parse(Console.ReadLine());
                var selectedItem = results.ElementAt(index);
                OutputObject("", selectedItem);
                
                while(true)
                {
                    Console.Write("Action(ViewImage, GetDetails, Search): ");
                    var action = Console.ReadLine();
                    if (action.Equals("ViewImage", StringComparison.OrdinalIgnoreCase))
                    {
                        var image = await imageClient.DownloadImageAsync(selectedItem.Image);
                        var directoryPath = Path.Combine(Path.GetTempPath(), "HuaHuaCaoCao");
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
                        var path = Path.Combine(directoryPath, selectedItem.PID.Replace(" ", "_") + ".jpg");
                        File.WriteAllBytes(path, image.Content);
                        Console.WriteLine("Image saved to " + path);
                    }
                    else if (action.Equals("GetDetails", StringComparison.OrdinalIgnoreCase))
                    {
                        var details = await plantClient.GetDetailsAsync(selectedItem.PID);
                        OutputObject("", details);
                    }
                    else if (action.Equals("Search", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }

            }
        }

        static void OutputObject(string prefix, object item)
        {
            foreach (var property in item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var value = property.GetValue(item);
                if (property.PropertyType == typeof(string))
                {
                    Console.WriteLine(prefix + property.Name + ": " + value);
                }
                else if (property.PropertyType == typeof(int))
                {
                    Console.WriteLine(prefix + property.Name + ": " + value);
                }
                else
                {
                    OutputObject(property.Name + ".", property.GetValue(item));
                }
            }
        }

        static string GetSetting(Settings setting)
        {
            var defaultValue = (string)Enum<Settings>.GetAttribute<DefaultValueAttribute>(setting)?.Value;
            if (string.IsNullOrEmpty(defaultValue))
            {
                Console.Write($"{setting.ToString()}: ");
                var value = Console.ReadLine();
                return value;
            }
            else
            {
                Console.Write($"{setting.ToString()}({defaultValue}): ");
                var value = Console.ReadLine();
                return string.IsNullOrEmpty(value) ? defaultValue : value;
            }
        }
    }
}
