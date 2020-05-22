namespace MiFloraGateway.Database
{
    public class PlantParameters
    {
        public int PlantId { get; set; }
        public Plant Plant { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public Range EnvironmentHumidity { get; set; }

        /// <summary>
        /// Lux
        /// </summary>
        public Range LightLux { get; set; }

        /// <summary>
        /// ????
        /// </summary>
        public Range LightMmol { get; set; }

        /// <summary>
        /// µS/cm
        /// </summary>
        public Range SoilFertility { get; set; }

        /// <summary>
        /// %
        /// </summary>
        public Range SoilHumidity { get; set; }

        /// <summary>
        /// C°
        /// </summary>
        public Range Temperature { get; set; }
    }
}
