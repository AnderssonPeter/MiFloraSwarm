using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MiFloraGateway.Sensors
{
    public interface IDetectSensorCommand
    {
        Task<IEnumerable<int>> CommandAsync();
    }
}
