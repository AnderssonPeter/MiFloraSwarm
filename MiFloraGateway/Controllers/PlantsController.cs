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

namespace MiFloraGateway.Controllers
{
    public class PlantsController : ODataController
    {
        private readonly DatabaseContext databaseContext;

        public PlantsController(DatabaseContext databaseContext) => this.databaseContext = databaseContext;

        
        [EnableQuery]
        public IQueryable<Plant> Get() => databaseContext.Plants;

        [EnableQuery]
        public SingleResult<Plant> Get([FromODataUri]int key) => SingleResult.Create(databaseContext.Plants.Where(x => x.Id == key));

        public async Task<IActionResult> Post([FromBody]Plant plant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            databaseContext.Plants.Add(plant);
            await databaseContext.SaveChangesAsync();
            return Created(plant);
        }

        public async Task<IActionResult> Patch([FromODataUri]int key, [FromBody]Delta<Plant> plant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await databaseContext.Plants.FindAsync(key);
            plant.Patch(entity);
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
            var movie = await databaseContext.Plants.FindAsync(key);
            if (movie == null)
            {
                return NotFound();
            }
            databaseContext.Plants.Remove(movie);
            await databaseContext.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool Exists(int key)
        {
            return databaseContext.Plants.Any(x => x.Id == key);
        }
    }
}