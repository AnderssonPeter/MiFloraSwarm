namespace MiFloraGateway
{
    public enum Settings
    {
        [Setting(typeof(string), "0 0 0 ? * *", true)]
        UpdateBatteryAndVersionCron,
        [Setting(typeof(string), "0 0 * ? * *", true)]
        UpdateValuesCron,
        [Setting(typeof(string), "MiFloraGateway", true)]
        MQTTClientId,
        [Setting(typeof(string), "", true)]
        MQTTServerAddress,
        [Setting(typeof(string), "", true)]
        MQTTUsername,
        [Setting(typeof(string), "", false)]
        MQTTPassword,
        [Setting(typeof(bool), false, true)]
        MQTTUseTLS,
        [Setting(typeof(string), "admin", true)]
        Username,
        [Setting(typeof(string), "", false)]
        Password

    }
}
