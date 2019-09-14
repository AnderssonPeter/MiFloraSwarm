namespace MiFloraGateway.Database
{
    public class PlantMaintenance
    {
        public int PlantId { get; set; }
        public Plant Plant { get; set; }
        public string Fertilization { get; set; }
        public string Pruning { get; set; }
        public string Size { get; set; }
        public string Soil { get; set; }
        public string Sunlight { get; set; }
        public string Watering { get; set; }
    }
}
