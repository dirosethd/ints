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
    public class ShipmentsController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;

        public ShipmentsController(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ShipmentDto>>> GetAll()
        {
            var shipments = await _shipmentService.GetAllAsync();
            return Ok(shipments);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ShipmentDto>> GetById(int id)
        {
            var shipment = await _shipmentService.GetByIdAsync(id);
            return shipment == null ? NotFound() : Ok(shipment);
        }

        [HttpPost]
        public async Task<ActionResult<ShipmentDto>> Create([FromBody] CreateShipmentRequest request)
        {
            try
            {
                var shipment = await _shipmentService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = shipment.Id }, shipment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateShipmentRequest request)
        {
            try
            {
                var updated = await _shipmentService.UpdateAsync(id, request);
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
            var deleted = await _shipmentService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
