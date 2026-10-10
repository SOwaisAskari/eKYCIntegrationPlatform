using System.ComponentModel.DataAnnotations;

namespace eKYC.Api.Contracts;

public sealed class KycSubmission
{
    [Required, StringLength(80, MinimumLength = 3)] public string CustomerReference { get; init; } = "";
    [Required, RegularExpression("^(NationalId|Passport|ResidencePermit)$")] public string IdentityType { get; init; } = "NationalId";
    [Required, StringLength(80, MinimumLength = 4)] public string IdentityNumber { get; init; } = "";
    public bool IsLegacy { get; init; }
    [Required, StringLength(100, MinimumLength = 2)] public string Requester { get; init; } = "Portfolio Demo";
}
