namespace MiFloraGateway.Database
{
    public class PlantParameters
    {
        public int PlantId { get; set; }
        public Plant Plant { get; set; } = null!;

        /// <summary>
        /// ?
        /// </summary>
        public Range EnvironmentHumidity { get; set; } = null!;

        /// <summary>
        /// Lux
        /// </summary>
        public Range LightLux { get; set; } = null!;

        /// <summary>
        /// ????
        /// </summary>
        public Range LightMmol { get; set; } = null!;

        /// <summary>
        /// µS/cm
        /// </summary>
        public Range SoilFertility { get; set; } = null!;

        /// <summary>
        /// %
        /// </summary>
        public Range SoilHumidity { get; set; } = null!;

        /// <summary>
        /// C°
        /// </summary>
        public Range Temperature { get; set; } = null!;
    }
}
