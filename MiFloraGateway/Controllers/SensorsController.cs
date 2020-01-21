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
using System.Net.Http;
using Polly;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using MiFloraGateway.Devices;
using MiFloraGateway.Sensors;

namespace MiFloraGateway.Controllers
{
    public class SensorsController : ODataController
    {
        private readonly DatabaseContext databaseContext;
        private readonly IJobManager jobManager;
        private readonly ILogger<SensorsController> logger;

        public SensorsController(DatabaseContext databaseContext, IJobManager jobManager, ILogger<SensorsController> logger)
        {
            this.databaseContext = databaseContext;
            this.jobManager = jobManager;
            this.logger = logger;
        }

        [EnableQuery]
        public IQueryable<Sensor> Get() => databaseContext.Sensors;

        [EnableQuery]
        public SingleResult<Sensor> Get([FromODataUri]int key) => SingleResult.Create(databaseContext.Sensors.Where(x => x.Id == key));

        public async Task<IActionResult> Post([FromBody]Sensor sensor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            databaseContext.Sensors.Add(sensor);
            await databaseContext.SaveChangesAsync();
            return Created(sensor);
        }

        public async Task<IActionResult> Patch([FromODataUri]int key, [FromBody]Delta<Sensor> sensor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await databaseContext.Sensors.FindAsync(key);
            sensor.Patch(entity);
            try
            {
                await databaseContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(key))
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
                if (!Exists(key))
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
            var movie = await databaseContext.Sensors.FindAsync(key);
            if (movie == null)
            {
                return NotFound();
            }
            databaseContext.Sensors.Remove(movie);
            await databaseContext.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        [HttpPut("Scan")]
        public async Task<IQueryable<Sensor>> ScanAsync()
        {
            logger.LogTrace("ScanAsync");
            var ids = await jobManager.StartWaitAsync<IEnumerable<int>, IDetectSensorCommand>(dsc => dsc.CommandAsync(), HttpContext.RequestAborted).ConfigureAwait(false);            
            return databaseContext.Sensors.Where(x => ids.Contains(x.Id));
        }

        private bool Exists(int key)
        {
            return databaseContext.Sensors.Any(x => x.Id == key);
        }
    }
}