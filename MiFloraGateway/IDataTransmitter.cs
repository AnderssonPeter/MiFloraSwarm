using System;
using System.Threading;
using System.Threading.Tasks;

namespace MiFloraGateway
{
    public interface IDataTransmitter
    {
        Task SendAsync(string name, int light, float temperature, int moisture, int conductivity, int battery, Version version, CancellationToken cancellationToken);
    }
}
