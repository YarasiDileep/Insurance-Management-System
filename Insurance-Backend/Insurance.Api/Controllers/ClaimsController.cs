using Microsoft.AspNetCore.Mvc;
using Insurance.Core.Entities;
using Insurance.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ClaimsController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.Claims.ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _db.Claims.FindAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Claim dto)
    {
        dto.Id = Guid.NewGuid();
        dto.CreatedAt = DateTime.UtcNow;
        _db.Claims.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Claim dto)
    {
        var existing = await _db.Claims.FindAsync(id);
        if (existing == null) return NotFound();
        existing.ClaimNumber = dto.ClaimNumber;
        existing.DateOfLoss = dto.DateOfLoss;
        existing.Amount = dto.Amount;
        existing.Status = dto.Status;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _db.Claims.FindAsync(id);
        if (existing == null) return NotFound();
        _db.Claims.Remove(existing);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
