using eKYC.Api.Contracts;
using eKYC.Api.Data;
using eKYC.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eKYC.Api.Controllers;

[ApiController]
[Route("api/consent")]
public sealed class ConsentController(RequestService service, DemoDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(await db.Consents.OrderByDescending(x => x.UpdatedAtUtc).ToListAsync(ct));

    [HttpPost]
    public async Task<IActionResult> Grant(ConsentRequest request, CancellationToken ct) =>
        Ok(await service.GrantConsentAsync(request, ct));

    [HttpPost("{id:int}/revoke")]
    public async Task<IActionResult> Revoke(int id, CancellationToken ct)
    {
        var result = await service.RevokeConsentAsync(id, ct);
        return result is null ? NotFound(new { message = "Consent record not found." }) : Ok(result);
    }
}
