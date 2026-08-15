namespace Insurance.Api.Services;

public interface IPaymentGateway
{
    // Process a payment - in real life this would call external provider
    Task<PaymentResult> ProcessPaymentAsync(Guid paymentId, decimal amount, string method, string reference, CancellationToken cancellationToken = default);
}

public record PaymentResult(bool Success, string TransactionId, string Message);
