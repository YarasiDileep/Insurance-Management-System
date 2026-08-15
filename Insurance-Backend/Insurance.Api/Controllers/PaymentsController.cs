using Microsoft.AspNetCore.Mvc;
using Insurance.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize]
public class PaymentsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public PaymentsController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.Payments.Select(p => new Insurance.Api.DTOs.PaymentDto(p.Id, p.PolicyId, p.CustomerId, p.Amount, p.PaidAt, p.Method, p.Reference)).ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _db.Payments.Where(p => p.Id == id).Select(p => new Insurance.Api.DTOs.PaymentDto(p.Id, p.PolicyId, p.CustomerId, p.Amount, p.PaidAt, p.Method, p.Reference)).FirstOrDefaultAsync();
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Create([FromBody] Insurance.Api.DTOs.CreatePaymentDto dto)
    {
        // business validations
        var policy = await _db.Policies.FindAsync(dto.PolicyId);
        if (policy == null) return BadRequest("Policy not found");
        if (policy.CustomerId != dto.CustomerId) return BadRequest("Policy does not belong to the specified customer");
        var now = DateTime.UtcNow.Date;
        if (policy.StartDate > now || policy.EndDate < now) return BadRequest("Policy is not active");

        var entity = new Insurance.Core.Entities.Payment
        {
            Id = Guid.NewGuid(),
            PolicyId = dto.PolicyId,
            CustomerId = dto.CustomerId,
            Amount = dto.Amount,
            Method = dto.Method,
            Reference = dto.Reference,
            PaidAt = DateTime.UtcNow
        };
        _db.Payments.Add(entity);
        await _db.SaveChangesAsync();

        // process through payment gateway (mock)
        var gateway = HttpContext.RequestServices.GetRequiredService<Insurance.Api.Services.IPaymentGateway>();
        var paymentResult = await gateway.ProcessPaymentAsync(entity.Id, entity.Amount, entity.Method, entity.Reference ?? string.Empty);
        // in a real system we'd update payment status. For now we log and return gateway transaction id
        var result = new Insurance.Api.DTOs.PaymentDto(entity.Id, entity.PolicyId, entity.CustomerId, entity.Amount, entity.PaidAt, entity.Method, paymentResult.TransactionId);
        return CreatedAtAction(nameof(Get), new { id = entity.Id }, result);
    }

    [HttpDelete("{id:guid}")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _db.Payments.FindAsync(id);
        if (existing == null) return NotFound();
        _db.Payments.Remove(existing);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
