using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.DTOs;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FirmwareController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FirmwareController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var firmwares = await _context.Firmware
                .Include(f => f.DeviceType)
                .Select(f => new FirmwareDto
                {
                    FirmwareId = f.FirmwareId,
                    Version = f.Version,
                    Status = f.Status,
                    ReleasedAt = f.ReleasedAt,
                    DeviceTypeId = f.DeviceTypeId,
                    DeviceTypeName = f.DeviceType.Name
                }).ToListAsync();

            return Ok(firmwares);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var firmware = await _context.Firmware
                .Include(f => f.DeviceType)
                .Where(f => f.FirmwareId == id)
                .Select(f => new FirmwareDto
                {
                    FirmwareId = f.FirmwareId,
                    Version = f.Version,
                    Status = f.Status,
                    ReleasedAt = f.ReleasedAt,
                    DeviceTypeId = f.DeviceTypeId,
                    DeviceTypeName = f.DeviceType.Name
                }).FirstOrDefaultAsync();

            if (firmware == null)
                return NotFound($"Firmware with ID {id} not found.");

            return Ok(firmware);
        }

        [HttpGet("ByDeviceType/{deviceTypeId}")]
        public async Task<IActionResult> GetByDeviceType(int deviceTypeId)
        {
            var firmwares = await _context.Firmware
                .Include(f => f.DeviceType)
                .Where(f => f.DeviceTypeId == deviceTypeId)
                .Select(f => new FirmwareDto
                {
                    FirmwareId = f.FirmwareId,
                    Version = f.Version,
                    Status = f.Status,
                    ReleasedAt = f.ReleasedAt,
                    DeviceTypeId = f.DeviceTypeId,
                    DeviceTypeName = f.DeviceType.Name
                }).ToListAsync();

            if (!firmwares.Any())
                return NotFound($"No firmware found for DeviceType ID {deviceTypeId}.");

            return Ok(firmwares);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FirmwareInputDto dto)
        {
           if (!ModelState.IsValid)
               return BadRequest(ModelState);

            var deviceTypeExists = await _context.DeviceType
                .AnyAsync(dt => dt.DeviceTypeId == dto.DeviceTypeId);

            if (!deviceTypeExists)
                return BadRequest($"DeviceType with ID {dto.DeviceTypeId} does not exist.");

            var firmware = new Firmware
            {
               Version      = dto.Version,
               Status       = dto.Status,
               DeviceTypeId = dto.DeviceTypeId
            };

            _context.Firmware.Add(firmware);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = firmware.FirmwareId }, firmware);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FirmwareInputDto dto)
        {
           if (id != dto.FirmwareId)
              return BadRequest("ID mismatch.");

            var deviceTypeExists = await _context.DeviceType
              .AnyAsync(dt => dt.DeviceTypeId == dto.DeviceTypeId);

            if (!deviceTypeExists)
              return BadRequest($"DeviceType with ID {dto.DeviceTypeId} does not exist.");

            var firmware = await _context.Firmware.FindAsync(id);
            if (firmware == null)
              return NotFound($"Firmware with ID {id} not found.");

            firmware.Version      = dto.Version;
            firmware.Status       = dto.Status;
            firmware.DeviceTypeId = dto.DeviceTypeId;

            try
            {
               await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
               if (!await _context.Firmware.AnyAsync(f => f.FirmwareId == id))
                  return NotFound($"Firmware with ID {id} not found.");
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var firmware = await _context.Firmware.FindAsync(id);
            if (firmware == null)
                return NotFound($"Firmware with ID {id} not found.");

            var inUse = await _context.Device.AnyAsync(d => d.FirmwareId == id);
            if (inUse)
                return BadRequest("Cannot delete firmware that is currently assigned to a device.");

            _context.Firmware.Remove(firmware);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}