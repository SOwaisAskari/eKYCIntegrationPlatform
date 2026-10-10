using eKYC.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

[ApiController]
[Route("api/worker")]
public sealed class WorkerController(RequestService service) : ControllerBase
{
    [HttpGet("next")]
    public async Task<IActionResult> ClaimNext(CancellationToken ct)
    {
        var item = await service.ClaimNextAsync(ct);
        return item is null ? NoContent() : Ok(new { item.Id, item.IdentityType, item.IdentityNumber, item.RetryAttempts });
    }

    [HttpPost("{id:int}/complete")]
    public async Task<IActionResult> Complete(int id, [FromBody] WorkerCompletion completion, CancellationToken ct)
    {
        var item = await service.CompleteAsync(id, completion.Success, completion.Message ?? "", ct);
        return item is null ? NotFound() : Ok(new { item.Id, item.Status, item.RetryAttempts, item.Message });
    }
}

public sealed class WorkerCompletion
{
    public bool Success { get; init; }
    public string? Message { get; init; }
}
