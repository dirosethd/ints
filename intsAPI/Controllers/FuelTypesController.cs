using ints.Models;
using intsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace intsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FuelTypesController : ControllerBase
    {
        private readonly IFuelTypeService _fuelTypeService;

        public FuelTypesController(IFuelTypeService fuelTypeService)
        {
            _fuelTypeService = fuelTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<List<FuelTypeDto>>> GetAll()
        {
            var fuelTypes = await _fuelTypeService.GetAllAsync();
            return Ok(fuelTypes);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<FuelTypeDto>> GetById(int id)
        {
            var fuelType = await _fuelTypeService.GetByIdAsync(id);
            return fuelType == null ? NotFound() : Ok(fuelType);
        }

        [HttpPost]
        public async Task<ActionResult<FuelTypeDto>> Create([FromBody] CreateFuelTypeRequest request)
        {
            var fuelType = await _fuelTypeService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = fuelType.Id }, fuelType);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFuelTypeRequest request)
        {
            var updated = await _fuelTypeService.UpdateAsync(id, request);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _fuelTypeService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}