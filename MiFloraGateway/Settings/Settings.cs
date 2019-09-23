using System.ComponentModel;

namespace MiFloraGateway
{
    public enum Settings
    {
        [DefaultValue("0 0 0 ? * *")]
        UpdateBatteryAndVersionCron,
        [DefaultValue("0 0 * ? * *")]
        UpdateValuesCron,
        [DefaultValue("MiFloraGateway")]
        MQTTClientId,
        [DefaultValue("")]
        MQTTServerAddress,
        [DefaultValue("")]
        MQTTUsername,
        [DefaultValue("")]
        MQTTPassword,
        [DefaultValue(false)]
        MQTTUseTLS
    }
}
