using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.DTOs;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceTypeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DeviceTypeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var deviceTypes = await _context.DeviceType
                .Select(dt => new DeviceTypeDto
                {
                    DeviceTypeId = dt.DeviceTypeId,
                    Name = dt.Name,
                    Description = dt.Description,
                    IsActive = dt.IsActive,
                    CreatedAt = dt.CreatedAt
                }).ToListAsync();

            return Ok(deviceTypes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var deviceType = await _context.DeviceType
                .Where(dt => dt.DeviceTypeId == id)
                .Select(dt => new DeviceTypeDto
                {
                    DeviceTypeId = dt.DeviceTypeId,
                    Name = dt.Name,
                    Description = dt.Description,
                    IsActive = dt.IsActive,
                    CreatedAt = dt.CreatedAt
                }).FirstOrDefaultAsync();

            if (deviceType == null)
                return NotFound($"DeviceType with ID {id} not found.");

            return Ok(deviceType);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DeviceType deviceType)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.DeviceType.Add(deviceType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = deviceType.DeviceTypeId }, deviceType);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DeviceType deviceType)
        {
            if (id != deviceType.DeviceTypeId)
                return BadRequest("ID mismatch.");

            _context.Entry(deviceType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.DeviceType.AnyAsync(dt => dt.DeviceTypeId == id))
                    return NotFound($"DeviceType with ID {id} not found.");
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deviceType = await _context.DeviceType.FindAsync(id);
            if (deviceType == null)
                return NotFound($"DeviceType with ID {id} not found.");

            _context.DeviceType.Remove(deviceType);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}