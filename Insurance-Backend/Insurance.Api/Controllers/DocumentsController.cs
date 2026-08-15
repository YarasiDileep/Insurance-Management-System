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
    public async Task<IActionResult> Create()
    {
        var form = await Request.ReadFormAsync();
        var file = form.Files.FirstOrDefault();
        if (file == null) return BadRequest("No file uploaded");

        var customerIdStr = form["customerId"].FirstOrDefault();
        Guid customerId = Guid.TryParse(customerIdStr, out var cId) ? cId : Guid.Empty;
        Guid? policyId = null;
        var policyIdStr = form["policyId"].FirstOrDefault();
        if (Guid.TryParse(policyIdStr, out var pId)) policyId = pId;

        // validation: ensure customer exists
        var customer = await _db.Customers.FindAsync(customerId);
        if (customer == null) return BadRequest("Customer not found");
        if (policyId.HasValue)
        {
            var policy = await _db.Policies.FindAsync(policyId.Value);
            if (policy == null) return BadRequest("Policy not found");
            if (policy.CustomerId != customerId) return BadRequest("Policy does not belong to the customer");
        }

        // save file via storage service
        var storage = HttpContext.RequestServices.GetRequiredService<Insurance.Api.Services.IStorageService>();
        using var stream = file.OpenReadStream();
        var storagePath = await storage.SaveFileAsync(stream, file.FileName, file.ContentType);

        var entity = new Insurance.Core.Entities.Document
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            PolicyId = policyId,
            FileName = file.FileName,
            ContentType = file.ContentType ?? "application/octet-stream",
            Size = file.Length,
            StoragePath = storagePath,
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
        // delete file from storage too
        var storage = HttpContext.RequestServices.GetRequiredService<Insurance.Api.Services.IStorageService>();
        if (!string.IsNullOrWhiteSpace(existing.StoragePath))
        {
            await storage.DeleteFileAsync(existing.StoragePath);
        }
        _db.Documents.Remove(existing);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id)
    {
        var existing = await _db.Documents.FindAsync(id);
        if (existing == null) return NotFound();
        var storage = HttpContext.RequestServices.GetRequiredService<Insurance.Api.Services.IStorageService>();
        var stream = await storage.GetFileAsync(existing.StoragePath);
        if (stream == null) return NotFound();
        return File(stream, existing.ContentType, existing.FileName);
    }
}
