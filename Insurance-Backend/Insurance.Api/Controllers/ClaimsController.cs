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
        var items = await _db.Claims
            .Select(c => new Insurance.Api.DTOs.ClaimDto(c.Id, c.ClaimNumber, c.PolicyId, c.CustomerId, c.DateOfLoss, c.Amount, c.Status, c.CreatedAt))
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _db.Claims
            .Where(c => c.Id == id)
            .Select(c => new Insurance.Api.DTOs.ClaimDto(c.Id, c.ClaimNumber, c.PolicyId, c.CustomerId, c.DateOfLoss, c.Amount, c.Status, c.CreatedAt))
            .FirstOrDefaultAsync();
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Insurance.Api.DTOs.CreateClaimDto dto)
    {
        var entity = new Claim
        {
            Id = Guid.NewGuid(),
            ClaimNumber = dto.ClaimNumber,
            PolicyId = dto.PolicyId,
            CustomerId = dto.CustomerId,
            DateOfLoss = dto.DateOfLoss,
            Amount = dto.Amount,
            CreatedAt = DateTime.UtcNow
        };
        _db.Claims.Add(entity);
        await _db.SaveChangesAsync();
        var result = new Insurance.Api.DTOs.ClaimDto(entity.Id, entity.ClaimNumber, entity.PolicyId, entity.CustomerId, entity.DateOfLoss, entity.Amount, entity.Status, entity.CreatedAt);
        return CreatedAtAction(nameof(Get), new { id = entity.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Insurance.Api.DTOs.UpdateClaimDto dto)
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
