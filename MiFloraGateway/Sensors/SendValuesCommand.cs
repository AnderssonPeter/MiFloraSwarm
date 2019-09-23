using Hangfire.Server;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiFloraGateway.Sensors
{
    public interface ISendValuesCommand
    {
        Task CommandAsync(PerformContext context, int sensorId);
    }
    public class SendValuesCommand : ISendValuesCommand
    {
        private readonly IMqttClient mqttClient;

        public SendValuesCommand(IMqttClient mqttClient)
        {
            this.mqttClient = mqttClient;
        }

        public async Task CommandAsync(PerformContext context, int sensorId)
        {
            
        }
    }
}
