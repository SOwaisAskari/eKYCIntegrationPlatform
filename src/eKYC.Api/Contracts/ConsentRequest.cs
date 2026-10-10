using System.ComponentModel.DataAnnotations;

namespace eKYC.Api.Contracts;

public sealed class ConsentRequest
{
    [Required, StringLength(80, MinimumLength = 3)] public string CustomerReference { get; init; } = "";
    [Required, StringLength(80, MinimumLength = 3)] public string Purpose { get; init; } = "KycProcessing";
}
