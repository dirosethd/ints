using ints.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace intsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FuelTypesController : ControllerBase
    {
        private readonly IntsContext _db;

        public FuelTypesController(IntsContext db)
        {
            _db = db;
        }

       
        [HttpGet]
        public async Task<ActionResult<List<FuelType>>> GetAll()
        {
            var list = await _db.FuelTypes
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return Ok(list);
        }

        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<FuelType>> GetById(int id)
        {
            var item = await _db.FuelTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return item == null ? NotFound() : Ok(item);
        }

       
        [HttpPost]
        public async Task<ActionResult<FuelType>> Create([FromBody] FuelType fuelType)
        {
            _db.FuelTypes.Add(fuelType);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = fuelType.Id }, fuelType);
        }

        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] FuelType fuelType)
        {
            if (id != fuelType.Id)
                return BadRequest("id и fuelType.Id должны совпадать");

            var exists = await _db.FuelTypes.AnyAsync(x => x.Id == id);
            if (!exists) return NotFound();

            _db.Entry(fuelType).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return NoContent();
        }

   
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.FuelTypes.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) return NotFound();

            _db.FuelTypes.Remove(item);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}