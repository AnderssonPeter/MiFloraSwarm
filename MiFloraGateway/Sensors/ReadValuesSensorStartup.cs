using Hangfire;
using MiFloraGateway.Settings;

namespace MiFloraGateway.Sensors
{
    public class ReadValuesSensorStartup : IRunOnStartup
    {
        private readonly IRecurringJobManager recurringJobManager;
        private readonly ISettingsManager settingsManager;
        
        public ReadValuesSensorStartup(IRecurringJobManager recurringJobManager, ISettingsManager settingsManager)
        {
            this.recurringJobManager = recurringJobManager;
            this.settingsManager = settingsManager;
        }

        public void Initialize()
        {

            UpdateSchedule(settingsManager.Get(Settings.UpdateValuesCron));
            settingsManager.WatchForChanges((_, cronExpression) => UpdateSchedule(cronExpression), Settings.UpdateValuesCron);
        }

        private void UpdateSchedule(string cronExpression)
        {
            recurringJobManager.AddOrUpdate<IReadValuesCommand>("ReadValues", c => c.CommandAsync(null), cronExpression);
        }
    }
}
