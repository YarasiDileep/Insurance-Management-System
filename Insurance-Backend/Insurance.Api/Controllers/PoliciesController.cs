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
        var items = await _db.Policies
            .Select(p => new Insurance.Api.DTOs.PolicyDto(p.Id, p.PolicyNumber, p.CustomerId, p.StartDate, p.EndDate, p.Premium, p.Status, p.CreatedAt))
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _db.Policies
            .Where(p => p.Id == id)
            .Select(p => new Insurance.Api.DTOs.PolicyDto(p.Id, p.PolicyNumber, p.CustomerId, p.StartDate, p.EndDate, p.Premium, p.Status, p.CreatedAt))
            .FirstOrDefaultAsync();
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Insurance.Api.DTOs.CreatePolicyDto dto)
    {
        var entity = new Policy
        {
            Id = Guid.NewGuid(),
            PolicyNumber = dto.PolicyNumber,
            CustomerId = dto.CustomerId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Premium = dto.Premium,
            CreatedAt = DateTime.UtcNow
        };
        _db.Policies.Add(entity);
        await _db.SaveChangesAsync();
        var result = new Insurance.Api.DTOs.PolicyDto(entity.Id, entity.PolicyNumber, entity.CustomerId, entity.StartDate, entity.EndDate, entity.Premium, entity.Status, entity.CreatedAt);
        return CreatedAtAction(nameof(Get), new { id = entity.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Insurance.Api.DTOs.UpdatePolicyDto dto)
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
