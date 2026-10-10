using System.ComponentModel.DataAnnotations;

namespace eKYC.Api.Models;

public sealed class ConsentRecord
{
    public int Id { get; set; }
    [Required, MaxLength(80)] public string CustomerReference { get; set; } = "";
    [Required, MaxLength(80)] public string Purpose { get; set; } = "KycProcessing";
    public bool IsGranted { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
