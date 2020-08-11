using Microsoft.AspNetCore.Mvc;
using MiFlora.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreFlora.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SensorsController : ControllerBase
    {

        [HttpGet("scan")]
        public IEnumerable<SensorInfo> Scan()
        {
            return new SensorInfo[] { 
                new SensorInfo {
                    MACAddress = "",
                    Name = "",
                    Rssi = 10
                } 
            };
        }
    }
}
