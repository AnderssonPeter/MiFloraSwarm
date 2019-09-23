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

            UpdateSchedule(settingsManager.Get(Settings.UpdateBatteryAndVersionCron));
            settingsManager.WatchForChanges((_, cronExpression) => UpdateSchedule(cronExpression), Settings.UpdateBatteryAndVersionCron);
        }

        private void UpdateSchedule(string cronExpression)
        {
            recurringJobManager.AddOrUpdate<IReadBatteryAndFirmwareCommand>("ReadBatteryAndFirmwareVersion", c => c.CommandAsync(null), cronExpression);
        }
    }
}
