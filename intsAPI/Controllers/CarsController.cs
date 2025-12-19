using ints.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace intsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly IntsContext _db;

        public CarsController(IntsContext db)
        {
            _db = db;
        }

        // GET: api/cars
        [HttpGet]
        public async Task<ActionResult<List<Car>>> GetAll()
        {
            var list = await _db.Cars
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return Ok(list);
        }

        // GET: api/cars/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Car>> GetById(int id)
        {
            var car = await _db.Cars
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return car == null ? NotFound() : Ok(car);
        }

        // POST: api/cars
        [HttpPost]
        public async Task<ActionResult<Car>> Create([FromBody] Car car)
        {
            _db.Cars.Add(car);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = car.Id }, car);
        }

        // PUT: api/cars/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Car car)
        {
            if (id != car.Id)
                return BadRequest("id и car.Id должны совпадать");

            var exists = await _db.Cars.AnyAsync(x => x.Id == id);
            if (!exists) return NotFound();

            _db.Entry(car).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/cars/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var car = await _db.Cars.FirstOrDefaultAsync(x => x.Id == id);
            if (car == null) return NotFound();

            _db.Cars.Remove(car);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
