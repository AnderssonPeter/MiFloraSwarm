using System;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Exceptions;
using MQTTnet.Extensions.ManagedClient;
using Nito.AsyncEx;

namespace MiFloraGateway
{


    public class DataTransmitter : IRunOnStartup, IDataTransmitter
    {
        private readonly ISettingsManager settingsManager;
        private readonly ILogger<DataTransmitter> logger;
        private readonly AsyncLock asyncLock = new AsyncLock();
        private IManagedMqttClient? client;
        private bool hasSettingsChanged = true;

        public DataTransmitter(ISettingsManager settingsManager, ILogger<DataTransmitter> logger)
        {
            this.settingsManager = settingsManager;
            this.logger = logger;
        }

        public async Task SendAsync(string name, int light, float temperature, int moisture, int conductivity, int battery, Version version, CancellationToken cancellationToken)
        {
            logger.LogTrace("SendAsync({name}, {light}, {temperature}, {moisture}, {conductivity}, {battery}, {version})", name, light, temperature, moisture, conductivity, battery, version);
            if (hasSettingsChanged)
            {
                client = await ConnectAsync(cancellationToken);
                hasSettingsChanged = false;
            }
            if (client == null)
            {
                throw new InvalidOperationException("Client hasn't been initialized yet, can't send data!");
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
            logger.LogTrace("ConnectAsync()");
            var clientId = settingsManager.Get<string>(Settings.MQTTClientId);
            var mqttServerAddress = settingsManager.Get<string>(Settings.MQTTServerAddress);
            var port = settingsManager.Get<int>(Settings.MQTTPort);
            var username = settingsManager.Get<string>(Settings.MQTTUsername);
            var password = settingsManager.Get<string>(Settings.MQTTPassword);
            var useTLS = settingsManager.Get<bool>(Settings.MQTTUseTLS);

            return await ConnectAsync(clientId, mqttServerAddress, port, username, password, useTLS, cancellationToken);
        }

        private async Task<IManagedMqttClient> ConnectAsync(string clientId, string mqttServerAddress, int port, string username, string password, bool useTLS, CancellationToken token)
        {
            var mqttClientFactory = new MqttFactory();
            var builder = new MqttClientOptionsBuilder();
            builder.WithTcpServer(mqttServerAddress, port);
            if (!string.IsNullOrWhiteSpace(clientId))
            {
                builder.WithClientId(clientId);
            }

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                builder.WithCredentials(username, password);
            }

            if (useTLS)
            {
                builder.WithTls();
            }

            var options = builder.Build();


            var managedOptions = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(options)
                .Build();

            //todo add a logger provided to the function CreateMqttClient
            var client = mqttClientFactory.CreateManagedMqttClient(new MqttNetLogger(logger));
            await client.StartAsync(managedOptions);
            await client.WaitForConnectAsync(2000, token);
            return client;
        }

        public void Initialize()
        {
            logger.LogTrace("Initialize()");
            settingsManager.WatchForChanges(onSettingChanged, Settings.MQTTClientId, Settings.MQTTServerAddress, Settings.MQTTPort, Settings.MQTTUsername, Settings.MQTTPassword, Settings.MQTTUseTLS);
        }

        private void onSettingChanged(Settings setting)
        {
            logger.LogTrace("onSettingChanged()");
            hasSettingsChanged = true;
        }

        public async Task<MqttVerificationResult> VerifySettingsAsync(string clientId, string mqttServerAddress, int port, string username, string password, bool useTLS, CancellationToken cancellationToken)
        {
            logger.LogTrace("VerifySettingsAsync({clientId}, {mqttServerAddress}, {port}, {password}, {useTLS})", clientId, mqttServerAddress, port, username, "****", useTLS);
            try
            {
                using (var client = await ConnectAsync(clientId, mqttServerAddress, port, username, password, useTLS, cancellationToken))
                {
                    var token = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, new CancellationTokenSource(1000).Token).Token;
                    await client.PingAsync(token);
                }

                return new MqttVerificationResult
                {
                    Success = true
                };
            }
            catch(MqttCommunicationTimedOutException ex)
            {
                logger.LogWarning(ex, "Exception occurred while validating settings!");
                return new MqttVerificationResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch(MqttCommunicationException ex)
            {
                logger.LogWarning(ex, "Exception occurred while validating settings!");
                return new MqttVerificationResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch(MqttProtocolViolationException ex)
            {
                logger.LogWarning(ex, "Exception occurred while validating settings!");
                return new MqttVerificationResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (TaskCanceledException ex)
            {
                logger.LogWarning(ex, "Exception occurred while validating settings!");
                return new MqttVerificationResult
                {
                    Success = false,
                    Message = "Failed to ping MQTT server"
                };
            }
        }
    }
}
