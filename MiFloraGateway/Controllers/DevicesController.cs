using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MiFloraGateway.Database;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using MiFloraGateway.Devices;
using Microsoft.Extensions.Logging;

namespace MiFloraGateway.Controllers
{
    public class DevicesController : ODataController
    {
        private readonly DatabaseContext databaseContext;
        private readonly IDetectDeviceCommand detectDeviceCommand;
        private readonly ILogger<DevicesController> logger;

        public DevicesController(DatabaseContext databaseContext, IDetectDeviceCommand detectDeviceCommand, ILogger<DevicesController> logger)
        {
            this.databaseContext = databaseContext;
            this.detectDeviceCommand = detectDeviceCommand;
            this.logger = logger;
        }

        [EnableQuery]
        public IQueryable<Device> Get() => databaseContext.Devices;
        
        [EnableQuery]
        public SingleResult<Device> Get([FromODataUri]int key) => SingleResult.Create(databaseContext.Devices.Where(x => x.Id == key));

        public async Task<IActionResult> Post([FromBody]Device device)
        {
            logger.LogTrace("Post");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            databaseContext.Devices.Add(device);
            await databaseContext.SaveChangesAsync();
            return Created(device);
        }

        public async Task<IActionResult> Patch([FromODataUri]int key, [FromBody]Delta<Device> device)
        {
            logger.LogTrace("Patch");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await databaseContext.Devices.FindAsync(key);
            device.Patch(entity);
            try
            {
                await databaseContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ExistsAsync(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(entity);
        }

        public async Task<IActionResult> Put([FromODataUri]int key, [FromBody] Device update)
        {
            logger.LogTrace("Put");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != update.Id)
            {
                return BadRequest();
            }
            databaseContext.Entry(update).State = EntityState.Modified;
            try
            {
                await databaseContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ExistsAsync(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(update);
        }

        public async Task<ActionResult> Delete([FromODataUri] int key)
        {
            logger.LogTrace("Delete");
            var movie = await databaseContext.Devices.FindAsync(key);
            if (movie == null)
            {
                return NotFound();
            }
            databaseContext.Devices.Remove(movie);
            await databaseContext.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        [HttpPut("Scan")]
        public async Task<IQueryable<Device>> Scan()
        {
            logger.LogTrace("ScanAsync");
            var ids = await detectDeviceCommand.ScanAsync(HttpContext.RequestAborted);            
            return databaseContext.Devices.Where(x => ids.Contains(x.Id));
        }

        private async Task<bool> ExistsAsync(int key)
        {
            return await databaseContext.Devices.AnyAsync(x => x.Id == key);
        }
    }
}