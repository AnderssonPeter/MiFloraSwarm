namespace MiFloraGateway.Database
{
    public class Plant
    {
        public int Id { get; set; }
        public string LatinName { get; set; } = null!;
        public string Alias { get; set; } = null!;
        public string Display { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;

        public PlantBasic Basic { get; set; } = null!;
        public PlantMaintenance Maintenance { get; set; } = null!;
        public PlantParameters Parameters { get; set; } = null!;
    }
}
