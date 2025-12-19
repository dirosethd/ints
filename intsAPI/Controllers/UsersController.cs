using ints.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace intsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IntsContext _db;

        public UsersController(IntsContext db)
        {
            _db = db;
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
           
            var users = await _db.Users
                .AsNoTracking()
                .Select(u => new { u.Id, u.Username, u.CreatedAtUtc })
                .OrderByDescending(u => u.Id)
                .ToListAsync();

            return Ok(users);
        }
    }
}
