using Microsoft.AspNetCore.Mvc;
using Insurance.Core.Entities;
using Insurance.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public CustomersController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.Customers.ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _db.Customers.FindAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Customer dto)
    {
        dto.Id = Guid.NewGuid();
        dto.CreatedAt = DateTime.UtcNow;
        _db.Customers.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Customer dto)
    {
        var existing = await _db.Customers.FindAsync(id);
        if (existing == null) return NotFound();
        existing.FirstName = dto.FirstName;
        existing.LastName = dto.LastName;
        existing.Email = dto.Email;
        existing.Phone = dto.Phone;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _db.Customers.FindAsync(id);
        if (existing == null) return NotFound();
        _db.Customers.Remove(existing);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
