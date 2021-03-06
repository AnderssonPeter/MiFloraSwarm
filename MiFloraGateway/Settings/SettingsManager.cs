﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiFloraGateway.Database;
using Nito.AsyncEx;

namespace MiFloraGateway
{
    public class SettingsManager : ISettingsManager, IRunOnStartup, IDisposable
    {
        AsyncLock locker = new AsyncLock();
        Dictionary<Settings, object?> storage = new Dictionary<Settings, object?>();
        Dictionary<Settings, List<Action<Settings>>> callbackBag = new Dictionary<Settings, List<Action<Settings>>>();
        private readonly DatabaseContext databaseContext;
        private readonly Dictionary<Type, ITypeConverter> typeConverters = new Dictionary<Type, ITypeConverter>();
        private readonly IServiceScope scope;

        public SettingsManager(IServiceScopeFactory serviceScopeFactory, IEnumerable<ITypeConverter> typeConverters)
        {
            scope = serviceScopeFactory.CreateScope();
            databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            foreach (ITypeConverter typeConverter in typeConverters)
            {
                this.typeConverters.Add(typeConverter.Type, typeConverter);
            }
            foreach (var setting in Enum<Settings>.GetValues())
            {
                callbackBag[setting] = new List<Action<Settings>>();
            }
        }

        private Type GetTypeForSetting(Settings setting)
        {
            return Enum<Settings>.GetAttribute<SettingAttribute>(setting).Type;
        }

        public T Get<T>(Settings setting)
        {
            if (typeof(T) != GetTypeForSetting(setting))
            {
                throw new ArgumentOutOfRangeException(nameof(T), "Wrong type for that setting!");
            }
            using (locker.Lock())
            {
                if (storage.TryGetValue(setting, out var value))
                {
                    return (T)value!;
                }
            }

            return (T)Enum<Settings>.GetAttribute<SettingAttribute>(setting).DefaultValue;
        }

        public async Task SetAsync<T>(Settings setting, T value, CancellationToken cancellationToken = default)
        {
            var type = GetTypeForSetting(setting);
            if (typeof(T) != type)
            {
                throw new ArgumentOutOfRangeException(nameof(T), "Wrong type for that setting!");
            }
            await SetUnsafeAsync(setting, value, cancellationToken);
        }

        public async Task SetUnsafeAsync(Settings setting, object value, CancellationToken cancellationToken = default)
        {
            var type = GetTypeForSetting(setting);
            if (value != null && value.GetType() != type)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Wrong type for that setting!");
            }
            using (await locker.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                var stringValue = type != typeof(string) ? this.typeConverters[type].ConvertToString(value) : (string?)value;
                if (storage.ContainsKey(setting))
                {
                    var entity = await this.databaseContext.Settings.SingleAsync(x => x.Key == setting, cancellationToken);
                    entity.Value = stringValue;
                    entity.LastChanged = DateTime.Now;
                    this.databaseContext.Settings.Update(entity);
                }
                else
                {
                    var entity = new Setting { Key = setting, Value = stringValue, LastChanged = DateTime.Now };
                    this.databaseContext.Settings.Add(entity);
                }
                await this.databaseContext.SaveChangesAsync(cancellationToken);

                storage[setting] = value;
            }

            if (callbackBag.TryGetValue(setting, out var callbacks))
            {
                callbacks.ForEach(c => {
                    c(setting);
                });
            }
        }

        public IDisposable WatchForChanges(Action<Settings> callback, params Settings[] settings)
        {
            using (locker.Lock())
            {
                foreach (var setting in settings)
                {
                    callbackBag[setting].Add(callback);
                }
            }
            return new Watcher(this, callback, settings);
        }

        public void Initialize()
        {
            foreach (var setting in databaseContext.Settings)
            {
                var type = GetTypeForSetting(setting.Key);
                var value = type != typeof(string) ? this.typeConverters[type].ConvertFromString(setting.Value) : (string?)setting.Value;
                storage.Add(setting.Key, value);
            }
        }

        public void Dispose()
        {
            scope.Dispose();
        }

        private class Watcher : IDisposable
        {
            private readonly SettingsManager settingsManager;
            private readonly Action<Settings> callback;
            private readonly Settings[] settings;

            public Watcher(SettingsManager settingsManager, Action<Settings> callback, Settings[] settings)
            {
                this.settingsManager = settingsManager;
                this.callback = callback;
                this.settings = settings;
            }

            public void Dispose()
            {
                using (settingsManager.locker.Lock())
                {
                    foreach (var settings in settings)
                    {
                        settingsManager.callbackBag[settings].Remove(callback);
                    }
                }
            }
        }
    }
}
