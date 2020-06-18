using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet.Extensions.ManagedClient;

namespace MiFloraGateway
{
    public static class MqttClientExtensionMethods
    {
        public static async Task WaitForConnectAsync(this IManagedMqttClient client, CancellationToken token = default)
        {
            var asyncManualResetEvent = new Nito.AsyncEx.AsyncManualResetEvent(false);
            var oldHandler = client.ConnectedHandler;
            client.UseConnectedHandler(e =>
            {
                asyncManualResetEvent.Set();
            });

            await asyncManualResetEvent.WaitAsync(token);
            client.ConnectedHandler = oldHandler;
        }

        public static Task WaitForConnectAsync(this IManagedMqttClient client, int timeoutMs, CancellationToken token = default)
        {
            return WaitForConnectAsync(client, CancellationTokenSource.CreateLinkedTokenSource(token, new CancellationTokenSource(timeoutMs).Token).Token);
        }

        public static Task WaitForConnectAsync(this IManagedMqttClient client, int timeoutMs)
        {
            return WaitForConnectAsync(client, new CancellationTokenSource(timeoutMs).Token);
        }
    }
}
