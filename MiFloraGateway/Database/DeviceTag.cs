namespace MiFloraGateway.Database
{
    public class DeviceTag
    {
        public int DeviceId { get; set; }
        public Device Device { get; set; } = null!;
        public string Tag { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
