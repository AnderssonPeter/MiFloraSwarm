using Hangfire;

namespace MiFloraGateway.Sensors
{
    public class ReadBatteryAndFirmwareStartup : IRunOnStartup
    {
        private readonly IRecurringJobManager recurringJobManager;
        private readonly ISettingsManager settingsManager;


        public ReadBatteryAndFirmwareStartup(IRecurringJobManager recurringJobManager, ISettingsManager settingsManager)
        {
            this.recurringJobManager = recurringJobManager;
            this.settingsManager = settingsManager;
        }

        public void Initialize()
        {

            UpdateSchedule();
            settingsManager.WatchForChanges(_ => UpdateSchedule(), Settings.UpdateBatteryAndVersionCron);
        }

        private void UpdateSchedule()
        {
            var cronExpression = settingsManager.Get<string>(Settings.UpdateBatteryAndVersionCron);
            recurringJobManager.AddOrUpdate<IReadBatteryAndFirmwareCommand>("ReadBatteryAndFirmwareVersion", c => c.CommandAsync(), cronExpression);
        }
    }
}
