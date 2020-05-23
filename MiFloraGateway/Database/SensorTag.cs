namespace MiFloraGateway.Database
{
    public class SensorTag
    {
        public int SensorId { get; set; }
        public Sensor Sensor { get; set; } = null!;
        public string Tag { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
