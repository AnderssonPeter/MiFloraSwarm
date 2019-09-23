using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MiFloraGateway
{
    public class SettingsManager : ISettingsManager
    {
        //todo: implement storage!
        object locker = new object();
        Dictionary<Settings, string> storage = new Dictionary<Settings, string>();
        Dictionary<Settings, List<Action<Settings, string>>> callbackBag = new Dictionary<Settings, List<Action<Settings, string>>>();

        public SettingsManager()
        {
            foreach(var setting in Enum<Settings>.GetValues())
            {
                callbackBag[setting] = new List<Action<Settings, string>>();
            }
        }

        public string Get(Settings setting)
        {
            lock (locker)
            {
                if (storage.TryGetValue(setting, out var value))
                {
                    return value;
                }
            }
            return (string)Enum<Settings>.GetAttribute<DefaultValueAttribute>(setting).Value;
        }

        public void Set(Settings setting, string value)
        {
            lock(locker)
            {
                storage[setting] = value;
                if (callbackBag.TryGetValue(setting, out var callbacks))
                {
                    callbacks.ForEach(c => c(setting,value));
                }
            }
        }

        public IDisposable WatchForChanges(Action<Settings, string> callback, params Settings[] settings)
        {
            lock (locker)
            {
                foreach (var setting in settings)
                {
                    callbackBag[setting].Add(callback);
                }
            }
            return new Watcher(this, callback, settings);
        }

        class Watcher : IDisposable
        {
            private readonly SettingsManager settingsManager;
            private readonly Action<Settings, string> callback;
            private readonly Settings[] settings;

            public Watcher(SettingsManager settingsManager, Action<Settings, string> callback, Settings[] settings)
            {
                this.settingsManager = settingsManager;
                this.callback = callback;
                this.settings = settings;
            }

            public void Dispose()
            {
                lock(settingsManager.locker)
                {
                    foreach(var settings in settings)
                    {
                        settingsManager.callbackBag[settings].Remove(callback);
                    }
                }
            }
        }
    }
}
