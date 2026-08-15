namespace Insurance.Api.Services;

// Very small mock payment gateway that simulates success/failure
public class MockPaymentGateway : IPaymentGateway
{
    private readonly ILogger<MockPaymentGateway> _log;
    public MockPaymentGateway(ILogger<MockPaymentGateway> log) => _log = log;

    public Task<PaymentResult> ProcessPaymentAsync(Guid paymentId, decimal amount, string method, string reference, CancellationToken cancellationToken = default)
    {
        // simulate processing delay
        _log.LogInformation("Mock processing payment {PaymentId} amount={Amount} method={Method}", paymentId, amount, method);
        var tx = "MOCKTX-" + Guid.NewGuid().ToString("N").Substring(0, 12);
        return Task.FromResult(new PaymentResult(true, tx, "Processed by mock gateway"));
    }
}
