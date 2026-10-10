using System.ComponentModel.DataAnnotations;

namespace eKYC.Api.Models;

public sealed class KycRequest
{
    public int Id { get; set; }
    [Required, MaxLength(80)] public string CustomerReference { get; set; } = "";
    [Required, MaxLength(30)] public string IdentityType { get; set; } = "NationalId";
    [Required, MaxLength(80)] public string IdentityNumber { get; set; } = "";
    public bool IsLegacy { get; set; }
    [Required, MaxLength(100)] public string Requester { get; set; } = "";
    [Required, MaxLength(20)] public string Status { get; set; } = "Pending";
    public int RetryAttempts { get; set; }
    public int MaxRetryAttempts { get; set; } = 3;
    [MaxLength(300)] public string Message { get; set; } = "Queued for demo processing.";
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public List<RequestEvent> Events { get; set; } = new();
}
