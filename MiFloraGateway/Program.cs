using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MiFloraGateway.GraphQL;

namespace MiFloraGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            if (args.Length == 1 && args[0].Equals("--GenerateClientCode", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Creating schema");
                var graphQLSchemaFile = @"ClientApp\src\app\api\graphql\schema.graphql";
                var schema = Schema.MakeSchema().GetGraphQLSchema();
                Console.WriteLine("Writing graphql schema to " + graphQLSchemaFile);
                File.WriteAllText(graphQLSchemaFile, schema);

                Console.WriteLine("Creating regexes");
                var regexes = GenerateRegexes();
                var regexesFile = @"ClientApp\src\app\regexes.ts";
                Console.WriteLine("Writing regexes to " + regexesFile);
                File.WriteAllText(regexesFile, regexes);


                Console.WriteLine("Creating settings");
                var settings = GenerateSettings();
                var settingsFile = @"ClientApp\src\app\settings.ts";
                Console.WriteLine("Writing settings to " + settingsFile);
                File.WriteAllText(settingsFile, settings);

                Console.WriteLine("Done");
                return;
            }
#endif
            CreateHostBuilder(args).Build().Run();
        }

        private static string GenerateRegexes()
        {
            var builder = new StringBuilder();
            builder.AppendLine("//Generated file do not make manual changes, they will be lost!");
            builder.AppendLine();
            foreach (var field in typeof(ValidationPatterns).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                builder.AppendLine($"export const {field.Name} = /{field.GetValue(null)}/");
            }
            return builder.ToString();
        }


        private static string GenerateSettings()
        {
            var builder = new StringBuilder();
            builder.AppendLine("//Generated file do not make manual changes, they will be lost!");
            builder.AppendLine();
            builder.AppendLine("export enum StringSettingType {");
            foreach (var setting in Enum<StringSettingType>.GetValues())
            {
                builder.AppendLine($"    {setting},");
            }
            builder.AppendLine("}");
            builder.AppendLine();


            builder.AppendLine(@"export class Setting<T> {
    constructor(public readonly name: string, public readonly isRequired: boolean, public readonly defaultValue: T) { }
}

export class NumberSetting extends Setting<number> { 
    constructor(name: string, isRequired: boolean, defaultValue: number) {
        super(name, isRequired, defaultValue);
    }
}

export class BooleanSetting extends Setting<boolean> { 
    constructor(name: string, isRequired: boolean, defaultValue: boolean) {
        super(name, isRequired, defaultValue);
    }
}

export class StringSetting extends Setting<string> {
    constructor(name: string, isRequired: boolean, defaultValue: string, public readonly stringType: StringSettingType = StringSettingType.Normal) {
        super(name, isRequired, defaultValue);
    }
}

export const SettingDefinitions = [");

            foreach (var setting in Enum<Settings>.GetValues())
            {
                var attribute = Enum<Settings>.GetAttribute<SettingAttribute>(setting);
                if (attribute is StringSettingAttribute stringSettingAttribute)
                {
                    builder.AppendLine($"    new StringSetting('{setting}', {attribute.IsRequired.ToString().ToLower()}, '{attribute.DefaultValue}', StringSettingType.{stringSettingAttribute.StringType}),");
                }
                else if (attribute.Type == typeof(bool))
                {
                    object defaultValue = attribute.DefaultValue;
                    if (defaultValue is bool boolValue)
                    {
                        defaultValue = boolValue.ToString().ToLower();
                    }
                    builder.AppendLine($"    new BooleanSetting('{setting}', {attribute.IsRequired.ToString().ToLower()}, {defaultValue}),");
                }
                else if (attribute.Type == typeof(int))
                {
                    object defaultValue = attribute.DefaultValue;
                    if (defaultValue is int intValue)
                    {
                        defaultValue = intValue.ToString().ToLower();
                    }
                    builder.AppendLine($"    new NumberSetting('{setting}', {attribute.IsRequired.ToString().ToLower()}, {defaultValue}),");
                }
                else
                {
                    object defaultValue = attribute.DefaultValue;
                    if (defaultValue is bool boolValue)
                    {
                        defaultValue = boolValue.ToString().ToLower();
                    }
                    builder.AppendLine($"    new Setting<{attribute.Type.Name}>('{setting}', {attribute.IsRequired.ToString().ToLower()}, {defaultValue}),");
                }
            }
            builder.AppendLine("]");
            return builder.ToString();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
