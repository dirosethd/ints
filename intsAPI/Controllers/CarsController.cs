using ints.Models;
using intsAPI.DTOs;
using intsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace intsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarsController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CarDto>>> GetAll()
        {
            var cars = await _carService.GetAllAsync();
            return Ok(cars);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CarDto>> GetById(int id)
        {
            var car = await _carService.GetByIdAsync(id);
            return car == null ? NotFound() : Ok(car);
        }

        [HttpPost]
        public async Task<ActionResult<CarDto>> Create([FromBody] CreateCarRequest request)
        {
            var car = await _carService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = car.Id }, car);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCarRequest request)
        {
            var updated = await _carService.UpdateAsync(id, request);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _carService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
