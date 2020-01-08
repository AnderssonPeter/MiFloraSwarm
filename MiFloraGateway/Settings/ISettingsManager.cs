using System;
using System.Threading;
using System.Threading.Tasks;

namespace MiFloraGateway
{
    public interface ISettingsManager
    {
        Task SetAsync<T>(Settings setting, T value, CancellationToken cancellationToken = default);
        T Get<T>(Settings setting);
        IDisposable WatchForChanges(Action<Settings> callback, params Settings[] settings);
    }
}
