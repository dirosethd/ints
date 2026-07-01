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
    public class DriversController : ControllerBase
    {
        private readonly IDriverService _driverService;

        public DriversController(IDriverService driverService)
        {
            _driverService = driverService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DriverDto>>> GetAll()
        {
            var drivers = await _driverService.GetAllAsync();
            return Ok(drivers);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DriverDto>> GetById(int id)
        {
            var driver = await _driverService.GetByIdAsync(id);
            return driver == null ? NotFound() : Ok(driver);
        }

        [HttpPost]
        public async Task<ActionResult<DriverDto>> Create([FromBody] CreateDriverRequest request)
        {
            try
            {
                var driver = await _driverService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = driver.Id }, driver);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDriverRequest request)
        {
            try
            {
                var updated = await _driverService.UpdateAsync(id, request);
                return updated ? NoContent() : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _driverService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
