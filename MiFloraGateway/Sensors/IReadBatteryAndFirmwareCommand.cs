using Hangfire.Server;
using System.Threading.Tasks;

namespace MiFloraGateway.Sensors
{
    public interface IReadBatteryAndFirmwareCommand
    {
        Task CommandAsync(PerformContext context);
    }
}
