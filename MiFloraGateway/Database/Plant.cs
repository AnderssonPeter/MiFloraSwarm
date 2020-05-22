namespace MiFloraGateway.Database
{
    public class Plant
    {
        public int Id { get; set; }
        public string LatinName { get; set; }
        public string Alias { get; set; }
        public string Display { get; set; }
        public string ImageUrl { get; set; }

        public PlantBasic Basic { get; set; }
        public PlantMaintenance Maintenance { get; set; }
        public PlantParameters Parameters { get; set; }
    }
}
