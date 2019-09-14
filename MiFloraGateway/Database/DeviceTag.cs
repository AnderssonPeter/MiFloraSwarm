﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiFloraGateway.Database
{
    public class DeviceTag
    {
        public int DeviceId { get; set; }
        public Device Device { get; set; }
        public string Tag { get; set; }
        public string Value { get; set; }
    }
}
