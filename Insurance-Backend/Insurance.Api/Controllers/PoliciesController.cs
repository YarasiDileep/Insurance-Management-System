using Microsoft.AspNetCore.Mvc;
using Insurance.Core.Entities;
using Insurance.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PoliciesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public PoliciesController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.Policies.ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _db.Policies.FindAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Policy dto)
    {
        dto.Id = Guid.NewGuid();
        dto.CreatedAt = DateTime.UtcNow;
        _db.Policies.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Policy dto)
    {
        var existing = await _db.Policies.FindAsync(id);
        if (existing == null) return NotFound();
        existing.PolicyNumber = dto.PolicyNumber;
        existing.StartDate = dto.StartDate;
        existing.EndDate = dto.EndDate;
        existing.Premium = dto.Premium;
        existing.Status = dto.Status;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _db.Policies.FindAsync(id);
        if (existing == null) return NotFound();
        _db.Policies.Remove(existing);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
