namespace MiFloraGateway.Database
{
    public class SensorTag
    {
        public int SensorId { get; set; }
        public Sensor Sensor { get; set; }
        public string Tag { get; set; }
        public string Value { get; set; }
    }
}
