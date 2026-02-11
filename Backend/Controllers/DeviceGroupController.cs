using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.DTOs;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceGroupController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DeviceGroupController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var groups = await _context.DeviceGroup
                .Include(g => g.ParentGroup)
                .Include(g => g.ChildGroups)
                .Select(g => new DeviceGroupDto
                {
                    GroupId = g.GroupId,
                    Name = g.Name,
                    CreatedAt = g.CreatedAt,
                    ParentGroupId = g.ParentGroupId,
                    ParentGroupName = g.ParentGroup != null ? g.ParentGroup.Name : null,
                    ChildGroups = g.ChildGroups.Select(c => new DeviceGroupDto
                    {
                        GroupId = c.GroupId,
                        Name = c.Name,
                        CreatedAt = c.CreatedAt,
                        ParentGroupId = c.ParentGroupId
                    }).ToList()
                }).ToListAsync();

            return Ok(groups);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var group = await _context.DeviceGroup
                .Include(g => g.ParentGroup)
                .Include(g => g.ChildGroups)
                .Where(g => g.GroupId == id)
                .Select(g => new DeviceGroupDto
                {
                    GroupId = g.GroupId,
                    Name = g.Name,
                    CreatedAt = g.CreatedAt,
                    ParentGroupId = g.ParentGroupId,
                    ParentGroupName = g.ParentGroup != null ? g.ParentGroup.Name : null,
                    ChildGroups = g.ChildGroups.Select(c => new DeviceGroupDto
                    {
                        GroupId = c.GroupId,
                        Name = c.Name,
                        CreatedAt = c.CreatedAt,
                        ParentGroupId = c.ParentGroupId
                    }).ToList()
                }).FirstOrDefaultAsync();

            if (group == null)
                return NotFound($"Group with ID {id} not found.");

            return Ok(group);
        }

        [HttpGet("TopLevel")]
        public async Task<IActionResult> GetTopLevel()
        {
            var groups = await _context.DeviceGroup
                .Include(g => g.ChildGroups)
                .Where(g => g.ParentGroupId == null)
                .Select(g => new DeviceGroupDto
                {
                    GroupId = g.GroupId,
                    Name = g.Name,
                    CreatedAt = g.CreatedAt,
                    ParentGroupId = g.ParentGroupId,
                    ChildGroups = g.ChildGroups.Select(c => new DeviceGroupDto
                    {
                        GroupId = c.GroupId,
                        Name = c.Name,
                        CreatedAt = c.CreatedAt,
                        ParentGroupId = c.ParentGroupId
                    }).ToList()
                }).ToListAsync();

            return Ok(groups);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DeviceGroup group)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (group.ParentGroupId.HasValue)
            {
                var parentExists = await _context.DeviceGroup
                    .AnyAsync(g => g.GroupId == group.ParentGroupId.Value);

                if (!parentExists)
                    return BadRequest($"Parent group with ID {group.ParentGroupId} does not exist.");
            }

            _context.DeviceGroup.Add(group);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = group.GroupId }, group);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DeviceGroup group)
        {
            if (id != group.GroupId)
                return BadRequest("ID mismatch.");

            if (group.ParentGroupId.HasValue)
            {
                var parentExists = await _context.DeviceGroup
                    .AnyAsync(g => g.GroupId == group.ParentGroupId.Value);

                if (!parentExists)
                    return BadRequest($"Parent group with ID {group.ParentGroupId} does not exist.");

                if (group.ParentGroupId == id)
                    return BadRequest("A group cannot be its own parent.");
            }

            _context.Entry(group).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.DeviceGroup.AnyAsync(g => g.GroupId == id))
                    return NotFound($"Group with ID {id} not found.");
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var group = await _context.DeviceGroup.FindAsync(id);
            if (group == null)
                return NotFound($"Group with ID {id} not found.");

            var hasChildren = await _context.DeviceGroup.AnyAsync(g => g.ParentGroupId == id);
            if (hasChildren)
                return BadRequest("Cannot delete a group that has child groups.");

            var hasDevices = await _context.Device.AnyAsync(d => d.GroupId == id);
            if (hasDevices)
                return BadRequest("Cannot delete a group that has devices assigned to it.");

            _context.DeviceGroup.Remove(group);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}