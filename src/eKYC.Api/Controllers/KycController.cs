using eKYC.Api.Contracts;
using eKYC.Api.Data;
using eKYC.Api.Models;
using eKYC.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eKYC.Api.Controllers;

[ApiController]
[Route("api/kyc/requests")]
public sealed class KycController(RequestService service, DemoDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(await db.KycRequests.OrderByDescending(x => x.CreatedAtUtc).Select(x => new
        {
            x.Id, x.CustomerReference, x.IdentityType, x.IsLegacy, x.Requester, x.Status,
            x.RetryAttempts, x.MaxRetryAttempts, x.Message, x.CreatedAtUtc, x.UpdatedAtUtc
        }).ToListAsync(ct));

    [HttpPost]
    public async Task<IActionResult> Submit(KycSubmission submission, CancellationToken ct)
    {
        try
        {
            var item = await service.SubmitAsync(submission, ct);
            return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        var item = await db.KycRequests.Include(x => x.Events).FirstOrDefaultAsync(x => x.Id == id, ct);
        return item is null ? NotFound() : Ok(item);
    }
}
