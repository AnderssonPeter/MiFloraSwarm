using Hangfire;

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
            UpdateSchedule();
            settingsManager.WatchForChanges(_ => UpdateSchedule());
        }

        private void UpdateSchedule()
        {
            var cronExpression = settingsManager.Get<string>(Settings.UpdateValuesCron);
            recurringJobManager.AddOrUpdate<IReadValuesCommand>("ReadValues", c => c.CommandAsync(), cronExpression);
        }
    }
}
