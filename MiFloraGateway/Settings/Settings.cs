namespace MiFloraGateway
{
    public enum Settings
    {
        [StringSetting(true, "0 0 0 ? * *", true, StringSettingType.Cron)]
        UpdateBatteryAndVersionCron,
        [StringSetting(true, "0 0 * ? * *", true, StringSettingType.Cron)]
        UpdateValuesCron,
        [StringSetting(true, "MiFloraGateway", true)]
        MQTTClientId,
        [StringSetting(true, "", true, StringSettingType.IPAddressOrHostname)]
        MQTTServerAddress,
        [Setting(typeof(int), true, 1883, true)]
        MQTTPort,
        [StringSetting(false, "", true)]
        MQTTUsername,
        [StringSetting(false, "", false, StringSettingType.Password)]
        MQTTPassword,
        [Setting(typeof(bool), true, false, true)]
        MQTTUseTLS
    }
}