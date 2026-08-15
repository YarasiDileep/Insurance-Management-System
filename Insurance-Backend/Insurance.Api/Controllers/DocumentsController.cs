using Microsoft.AspNetCore.Mvc;
using Insurance.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize]
public class DocumentsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public DocumentsController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.Documents.Select(d => new Insurance.Api.DTOs.DocumentDto(d.Id, d.CustomerId, d.PolicyId, d.FileName, d.ContentType, d.Size, d.UploadedAt, d.StoragePath)).ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _db.Documents.Where(d => d.Id == id).Select(d => new Insurance.Api.DTOs.DocumentDto(d.Id, d.CustomerId, d.PolicyId, d.FileName, d.ContentType, d.Size, d.UploadedAt, d.StoragePath)).FirstOrDefaultAsync();
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Create([FromBody] Insurance.Api.DTOs.CreateDocumentDto dto)
    {
        var entity = new Insurance.Core.Entities.Document
        {
            Id = Guid.NewGuid(),
            CustomerId = dto.CustomerId,
            PolicyId = dto.PolicyId,
            FileName = dto.FileName,
            ContentType = dto.ContentType,
            Size = dto.Size,
            StoragePath = dto.StoragePath,
            UploadedAt = DateTime.UtcNow
        };
        _db.Documents.Add(entity);
        await _db.SaveChangesAsync();
        var result = new Insurance.Api.DTOs.DocumentDto(entity.Id, entity.CustomerId, entity.PolicyId, entity.FileName, entity.ContentType, entity.Size, entity.UploadedAt, entity.StoragePath);
        return CreatedAtAction(nameof(Get), new { id = entity.Id }, result);
    }

    [HttpDelete("{id:guid}")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _db.Documents.FindAsync(id);
        if (existing == null) return NotFound();
        _db.Documents.Remove(existing);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
