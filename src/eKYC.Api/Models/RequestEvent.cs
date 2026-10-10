using System.ComponentModel.DataAnnotations;

namespace eKYC.Api.Models;

public sealed class RequestEvent
{
    public int Id { get; set; }
    public int KycRequestId { get; set; }
    public KycRequest? KycRequest { get; set; }
    [Required, MaxLength(30)] public string Status { get; set; } = "";
    [Required, MaxLength(300)] public string Message { get; set; } = "";
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
