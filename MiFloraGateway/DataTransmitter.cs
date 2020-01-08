using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiFloraGateway
{
    public class DataTransmitter : IRunOnStartup
    {
        private readonly ISettingsManager settingsManager;
        private readonly AsyncLock asyncLock = new AsyncLock();
        private IManagedMqttClient client;
        private bool hasSettingsChanged = true;

        public DataTransmitter(ISettingsManager settingsManager)
        {
            this.settingsManager = settingsManager;
        }

        public async Task SendAsync(string name, int light, float temperature, int moisture, int conductivity, int battery, Version version, CancellationToken cancellationToken)
        {
            if (hasSettingsChanged)
            {
                client = await ConnectAsync(cancellationToken);
                hasSettingsChanged = false;
            }
            var data = new { light, temperature, moisture, conductivity, battery, version = version.ToString() };
            var content = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            await client.PublishAsync(new MqttApplicationMessage
            {
                Topic = "miflora/" + name,
                ContentType = "json",
                QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce,
                Retain = true,
                Payload = Encoding.UTF8.GetBytes(content)
            }, cancellationToken);
        }

        private async Task<IManagedMqttClient> ConnectAsync(CancellationToken cancellationToken)
        {
            var mqttClientFactory = new MqttFactory();
            var builder = new MqttClientOptionsBuilder();
            var clientId = settingsManager.Get<string>(Settings.MQTTClientId);
            if (!string.IsNullOrWhiteSpace(clientId))
            {
                builder.WithClientId(clientId);
            }

            var username = settingsManager.Get<string>(Settings.MQTTUsername);
            var password = settingsManager.Get<string>(Settings.MQTTPassword);
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                builder.WithCredentials(username, password);
            }

            var useTLS = settingsManager.Get<bool>(Settings.MQTTUseTLS);
            if(useTLS)
            {
                builder.WithTls();
            }
            
            var options = builder.Build();


            var managedOptions = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(options)
                .Build();

            //todo add a logger provided to the function CreateMqttClient
            var client = mqttClientFactory.CreateManagedMqttClient();
            await client.StartAsync(managedOptions);
            return client;
        }

        public void Initialize()
        {
            settingsManager.WatchForChanges(onSettingChanged, Settings.MQTTClientId, Settings.MQTTServerAddress, Settings.MQTTUsername, Settings.MQTTPassword, Settings.MQTTUseTLS);
        }

        private void onSettingChanged(Settings setting)
        {
            hasSettingsChanged = true;
        }
    }
}
