using ints.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace intsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentsController : ControllerBase
    {
        private readonly IntsContext _db;

        public ShipmentsController(IntsContext db)
        {
            _db = db;
        }

       
        [HttpGet]
        public async Task<ActionResult<List<Shipment>>> GetAll()
        {
            var list = await _db.Shipments
                .AsNoTracking()
                .Include(s => s.Car)
                .Include(s => s.Driver)
                .Include(s => s.FuelType)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return Ok(list);
        }

       
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Shipment>> GetById(int id)
        {
            var shipment = await _db.Shipments
                .AsNoTracking()
                .Include(s => s.Car)
                .Include(s => s.Driver)
                .Include(s => s.FuelType)
                .FirstOrDefaultAsync(x => x.Id == id);

            return shipment == null ? NotFound() : Ok(shipment);
        }

        [HttpPost]
        public async Task<ActionResult<Shipment>> Create([FromBody] Shipment shipment)
        {
            
            if (!await _db.Cars.AnyAsync(x => x.Id == shipment.CarId))
                return BadRequest("CarId не существует");

            if (!await _db.Drivers.AnyAsync(x => x.Id == shipment.DriverId))
                return BadRequest("DriverId не существует");

            if (!await _db.FuelTypes.AnyAsync(x => x.Id == shipment.FuelTypeId))
                return BadRequest("FuelTypeId не существует");

            _db.Shipments.Add(shipment);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = shipment.Id }, shipment);
        }

        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Shipment shipment)
        {
            if (id != shipment.Id)
                return BadRequest("id и shipment.Id должны совпадать");

            var exists = await _db.Shipments.AnyAsync(x => x.Id == id);
            if (!exists) return NotFound();

            if (!await _db.Cars.AnyAsync(x => x.Id == shipment.CarId))
                return BadRequest("CarId не существует");

            if (!await _db.Drivers.AnyAsync(x => x.Id == shipment.DriverId))
                return BadRequest("DriverId не существует");

            if (!await _db.FuelTypes.AnyAsync(x => x.Id == shipment.FuelTypeId))
                return BadRequest("FuelTypeId не существует");

            _db.Entry(shipment).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return NoContent();
        }

       
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var shipment = await _db.Shipments.FirstOrDefaultAsync(x => x.Id == id);
            if (shipment == null) return NotFound();

            _db.Shipments.Remove(shipment);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}