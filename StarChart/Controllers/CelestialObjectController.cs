using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(o => o.Id == id);
            if (celestialObject == null)
            {
                return NotFound();
            }
            celestialObject.Satellites = new List<CelestialObject>();
            celestialObject.Satellites.Add(celestialObject);

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(o => o.Name == name).ToList();
            if (celestialObjects.Count == 0)
            {
                return NotFound();
            }

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = new List<CelestialObject>();
                celestialObject.Satellites.Add(celestialObject);
            }

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = new List<CelestialObject>();
                celestialObject.Satellites.Add(celestialObject);
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute(
                "GetById",
                new { Id = celestialObject.Id },
                celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var celestialObjectFromStore = _context.CelestialObjects.FirstOrDefault(o => o.Id == id);
            if (celestialObjectFromStore == null)
            {
                return NotFound();
            }
            celestialObjectFromStore.Name = celestialObject.Name;
            celestialObjectFromStore.OrbitalPeriod = celestialObject.OrbitalPeriod;
            celestialObjectFromStore.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.CelestialObjects.Update(celestialObjectFromStore);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObjectFromStore = _context.CelestialObjects.FirstOrDefault(o => o.Id == id);
            if (celestialObjectFromStore == null)
            {
                return NotFound();
            }
            celestialObjectFromStore.Name = name;
            _context.CelestialObjects.Update(celestialObjectFromStore);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objects = _context.CelestialObjects.Where(
                o => o.Id == id || o.OrbitedObjectId == id).ToList();
            if (objects.Count == 0)
            {
                return NotFound();
            }
            _context.CelestialObjects.RemoveRange(objects);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
