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
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.Customers
            .Select(c => new Insurance.Api.DTOs.CustomerDto(c.Id, c.FirstName, c.LastName, c.Email, c.Phone, c.DateOfBirth, c.CreatedAt))
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _db.Customers
            .Where(c => c.Id == id)
            .Select(c => new Insurance.Api.DTOs.CustomerDto(c.Id, c.FirstName, c.LastName, c.Email, c.Phone, c.DateOfBirth, c.CreatedAt))
            .FirstOrDefaultAsync();
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> Create([FromBody] Insurance.Api.DTOs.CreateCustomerDto dto)
    {
        var entity = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            DateOfBirth = dto.DateOfBirth,
            CreatedAt = DateTime.UtcNow
        };
        _db.Customers.Add(entity);
        await _db.SaveChangesAsync();
        var result = new Insurance.Api.DTOs.CustomerDto(entity.Id, entity.FirstName, entity.LastName, entity.Email, entity.Phone, entity.DateOfBirth, entity.CreatedAt);
        return CreatedAtAction(nameof(Get), new { id = entity.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] Insurance.Api.DTOs.UpdateCustomerDto dto)
    {
        var existing = await _db.Customers.FindAsync(id);
        if (existing == null) return NotFound();
        existing.FirstName = dto.FirstName;
        existing.LastName = dto.LastName;
        existing.Email = dto.Email;
        existing.Phone = dto.Phone;
        existing.DateOfBirth = dto.DateOfBirth;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _db.Customers.FindAsync(id);
        if (existing == null) return NotFound();
        _db.Customers.Remove(existing);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
