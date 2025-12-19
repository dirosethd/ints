using ints.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace intsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriversController : ControllerBase
    {
        private readonly IntsContext _db;

        public DriversController(IntsContext db)
        {
            _db = db;
        }

      
        [HttpGet]
        public async Task<ActionResult<List<Driver>>> GetAll()
        {
            var list = await _db.Drivers
                .AsNoTracking()
                .Include(d => d.Car)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return Ok(list);
        }

      
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Driver>> GetById(int id)
        {
            var driver = await _db.Drivers
                .AsNoTracking()
                .Include(d => d.Car)
                .FirstOrDefaultAsync(x => x.Id == id);

            return driver == null ? NotFound() : Ok(driver);
        }

      
        [HttpPost]
        public async Task<ActionResult<Driver>> Create([FromBody] Driver driver)
        {
           
            var carExists = await _db.Cars.AnyAsync(c => c.Id == driver.CarId);
            if (!carExists) return BadRequest("CarId не существует");

            _db.Drivers.Add(driver);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = driver.Id }, driver);
        }

       
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Driver driver)
        {
            if (id != driver.Id)
                return BadRequest("id и driver.Id должны совпадать");

            var exists = await _db.Drivers.AnyAsync(x => x.Id == id);
            if (!exists) return NotFound();

            var carExists = await _db.Cars.AnyAsync(c => c.Id == driver.CarId);
            if (!carExists) return BadRequest("CarId не существует");

            _db.Entry(driver).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return NoContent();
        }

       
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var driver = await _db.Drivers.FirstOrDefaultAsync(x => x.Id == id);
            if (driver == null) return NotFound();

            _db.Drivers.Remove(driver);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
