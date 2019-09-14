﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace MiFloraGateway.Database
{
    public class Sensor
    {
        public int Id { get; set; }
        public string MACAddress { get; set; }
        public string Name { get; set; }
        public Plant Plant { get; set; }
        public ICollection<DeviceSensorDistance> DeviceDistances { get; set; }
        public ICollection<SensorBatteryAndVersionReading> BatteryAndVersionReadings { get; set; }
        public ICollection<SensorDataReading> DataReadings { get; set; }
        public ICollection<SensorTag> Tags { get; set; }
        public ICollection<LogEntry> Logs { get; set; }
    }
}