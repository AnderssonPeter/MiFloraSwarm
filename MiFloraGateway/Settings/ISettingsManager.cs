using System;

namespace MiFloraGateway
{
    public interface ISettingsManager
    {
        void Set(Settings setting, string value);
        string Get(Settings setting);
        IDisposable WatchForChanges(Action<Settings, string> callback, params Settings[] settings);
    }
}
