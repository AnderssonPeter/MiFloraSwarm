﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiFloraGateway.Database
{
    public class SensorDataReading
    {
        public int SensorId { get; set; }
        public Sensor Sensor { get; set; }
        public DateTime When { get; set; }
        public int Moisture { get; set; }
        public int Brightness { get; set; }
        public float Temperature { get; set; }
        public int Conductivity { get; set; }
    }
}