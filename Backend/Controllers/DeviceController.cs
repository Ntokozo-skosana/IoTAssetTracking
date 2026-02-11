using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.DTOs;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DeviceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var devices = await _context.Device
                .Include(d => d.DeviceType)
                .Include(d => d.Firmware)
                .Include(d => d.Group)
                .Select(d => new DeviceDto
                {
                    DeviceId = d.DeviceId,
                    SerialNumber = d.SerialNumber,
                    Status = d.Status,
                    CreatedAt = d.CreatedAt,
                    DeviceTypeId = d.DeviceTypeId,
                    DeviceTypeName = d.DeviceType.Name,
                    FirmwareId = d.FirmwareId,
                    FirmwareVersion = d.Firmware.Version,
                    GroupId = d.GroupId,
                    GroupName = d.Group != null ? d.Group.Name : null
                }).ToListAsync();

            return Ok(devices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var device = await _context.Device
                .Include(d => d.DeviceType)
                .Include(d => d.Firmware)
                .Include(d => d.Group)
                .Where(d => d.DeviceId == id)
                .Select(d => new DeviceDto
                {
                    DeviceId = d.DeviceId,
                    SerialNumber = d.SerialNumber,
                    Status = d.Status,
                    CreatedAt = d.CreatedAt,
                    DeviceTypeId = d.DeviceTypeId,
                    DeviceTypeName = d.DeviceType.Name,
                    FirmwareId = d.FirmwareId,
                    FirmwareVersion = d.Firmware.Version,
                    GroupId = d.GroupId,
                    GroupName = d.Group != null ? d.Group.Name : null
                }).FirstOrDefaultAsync();

            if (device == null)
                return NotFound($"Device with ID {id} not found.");

            return Ok(device);
        }

        [HttpGet("ByGroup/{groupId}")]
        public async Task<IActionResult> GetByGroup(int groupId)
        {
            var groupExists = await _context.DeviceGroup.AnyAsync(g => g.GroupId == groupId);
            if (!groupExists)
                return NotFound($"Group with ID {groupId} not found.");

            var devices = await _context.Device
                .Include(d => d.DeviceType)
                .Include(d => d.Firmware)
                .Include(d => d.Group)
                .Where(d => d.GroupId == groupId)
                .Select(d => new DeviceDto
                {
                    DeviceId = d.DeviceId,
                    SerialNumber = d.SerialNumber,
                    Status = d.Status,
                    CreatedAt = d.CreatedAt,
                    DeviceTypeId = d.DeviceTypeId,
                    DeviceTypeName = d.DeviceType.Name,
                    FirmwareId = d.FirmwareId,
                    FirmwareVersion = d.Firmware.Version,
                    GroupId = d.GroupId,
                    GroupName = d.Group != null ? d.Group.Name : null
                }).ToListAsync();

            return Ok(devices);
        }

        [HttpGet("Unassigned")]
        public async Task<IActionResult> GetUnassigned()
        {
            var devices = await _context.Device
                .Include(d => d.DeviceType)
                .Include(d => d.Firmware)
                .Where(d => d.GroupId == null)
                .Select(d => new DeviceDto
                {
                    DeviceId = d.DeviceId,
                    SerialNumber = d.SerialNumber,
                    Status = d.Status,
                    CreatedAt = d.CreatedAt,
                    DeviceTypeId = d.DeviceTypeId,
                    DeviceTypeName = d.DeviceType.Name,
                    FirmwareId = d.FirmwareId,
                    FirmwareVersion = d.Firmware.Version,
                    GroupId = d.GroupId,
                    GroupName = null
                }).ToListAsync();

            return Ok(devices);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Device device)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var deviceTypeExists = await _context.DeviceType
                .AnyAsync(dt => dt.DeviceTypeId == device.DeviceTypeId);
            if (!deviceTypeExists)
                return BadRequest($"DeviceType with ID {device.DeviceTypeId} does not exist.");

            var firmware = await _context.Firmware
                .FirstOrDefaultAsync(f => f.FirmwareId == device.FirmwareId);
            if (firmware == null)
                return BadRequest($"Firmware with ID {device.FirmwareId} does not exist.");
            if (firmware.DeviceTypeId != device.DeviceTypeId)
                return BadRequest("Firmware does not belong to the specified DeviceType.");

            if (device.GroupId.HasValue)
            {
                var groupExists = await _context.DeviceGroup
                    .AnyAsync(g => g.GroupId == device.GroupId.Value);
                if (!groupExists)
                    return BadRequest($"Group with ID {device.GroupId} does not exist.");
            }

            var serialExists = await _context.Device
                .AnyAsync(d => d.SerialNumber == device.SerialNumber);
            if (serialExists)
                return BadRequest($"A device with serial number {device.SerialNumber} already exists.");

            _context.Device.Add(device);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = device.DeviceId }, device);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Device device)
        {
            if (id != device.DeviceId)
                return BadRequest("ID mismatch.");

            var deviceTypeExists = await _context.DeviceType
                .AnyAsync(dt => dt.DeviceTypeId == device.DeviceTypeId);
            if (!deviceTypeExists)
                return BadRequest($"DeviceType with ID {device.DeviceTypeId} does not exist.");

            var firmware = await _context.Firmware
                .FirstOrDefaultAsync(f => f.FirmwareId == device.FirmwareId);
            if (firmware == null)
                return BadRequest($"Firmware with ID {device.FirmwareId} does not exist.");
            if (firmware.DeviceTypeId != device.DeviceTypeId)
                return BadRequest("Firmware does not belong to the specified DeviceType.");

            if (device.GroupId.HasValue)
            {
                var groupExists = await _context.DeviceGroup
                    .AnyAsync(g => g.GroupId == device.GroupId.Value);
                if (!groupExists)
                    return BadRequest($"Group with ID {device.GroupId} does not exist.");
            }

            var serialExists = await _context.Device
                .AnyAsync(d => d.SerialNumber == device.SerialNumber && d.DeviceId != id);
            if (serialExists)
                return BadRequest($"A device with serial number {device.SerialNumber} already exists.");

            _context.Entry(device).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Device.AnyAsync(d => d.DeviceId == id))
                    return NotFound($"Device with ID {id} not found.");
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var device = await _context.Device.FindAsync(id);
            if (device == null)
                return NotFound($"Device with ID {id} not found.");

            _context.Device.Remove(device);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
