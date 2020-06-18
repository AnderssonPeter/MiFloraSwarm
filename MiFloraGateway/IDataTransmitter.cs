using System;
using System.Threading;
using System.Threading.Tasks;

namespace MiFloraGateway
{

    public class MqttVerificationResult
    {
        public bool Success
        { get; set; }

        public string? Message
        { get; set; }

    }
    public interface IDataTransmitter
    {
        Task<MqttVerificationResult> VerifySettingsAsync(string clientId, string mqttServerAddress, int port, string username, string password, bool useTLS, CancellationToken cancellationToken = default);
        Task SendAsync(string name, int light, float temperature, int moisture, int conductivity, int battery, Version version, CancellationToken cancellationToken = default);
    }
}
