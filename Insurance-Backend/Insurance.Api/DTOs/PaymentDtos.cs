using System;
using System.ComponentModel.DataAnnotations;

namespace Insurance.Api.DTOs;

public record PaymentDto(Guid Id, Guid PolicyId, Guid CustomerId, decimal Amount, DateTime PaidAt, string Method, string Reference);

public record CreatePaymentDto(
    [Required] Guid PolicyId,
    [Required] Guid CustomerId,
    [Required][Range(0.01, 79228162514264337593543950335.0)] decimal Amount,
    [Required][MaxLength(100)] string Method,
    [MaxLength(200)] string Reference
);

public record UpdatePaymentDto(
    [Required][Range(0.01, 79228162514264337593543950335.0)] decimal Amount,
    [Required][MaxLength(100)] string Method,
    [MaxLength(200)] string Reference
);
