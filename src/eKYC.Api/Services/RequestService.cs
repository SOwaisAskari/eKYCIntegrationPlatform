using eKYC.Api.Contracts;
using eKYC.Api.Data;
using eKYC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace eKYC.Api.Services;

public sealed class RequestService(DemoDbContext db)
{
    public async Task<object> GrantConsentAsync(ConsentRequest request, CancellationToken ct)
    {
        var record = await db.Consents.FirstOrDefaultAsync(x =>
            x.CustomerReference == request.CustomerReference && x.Purpose == request.Purpose, ct);
        if (record is null)
        {
            record = new ConsentRecord { CustomerReference = request.CustomerReference.Trim(), Purpose = request.Purpose.Trim() };
            db.Consents.Add(record);
        }
        record.IsGranted = true;
        record.UpdatedAtUtc = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
        return record;
    }

    public async Task<object?> RevokeConsentAsync(int id, CancellationToken ct)
    {
        var record = await db.Consents.FindAsync([id], ct);
        if (record is null) return null;
        record.IsGranted = false;
        record.UpdatedAtUtc = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
        return record;
    }

    public async Task<KycRequest> SubmitAsync(KycSubmission submission, CancellationToken ct)
    {
        var consent = await db.Consents.AnyAsync(x =>
            x.CustomerReference == submission.CustomerReference && x.Purpose == "KycProcessing" && x.IsGranted, ct);
        if (!consent)
            throw new InvalidOperationException("Active KycProcessing consent is required before submitting a request.");

        var item = new KycRequest
        {
            CustomerReference = submission.CustomerReference.Trim(),
            IdentityType = submission.IdentityType,
            IdentityNumber = submission.IdentityNumber.Trim(),
            IsLegacy = submission.IsLegacy,
            Requester = submission.Requester.Trim()
        };
        item.Events.Add(new RequestEvent { Status = item.Status, Message = item.Message });
        db.KycRequests.Add(item);
        await db.SaveChangesAsync(ct);
        return item;
    }

    public async Task<KycRequest?> ClaimNextAsync(CancellationToken ct)
    {
        var item = await db.KycRequests
            .Where(x => x.Status == "Pending" || x.Status == "Retry")
            .OrderBy(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(ct);
        if (item is null) return null;

        item.Status = "Processing";
        item.RetryAttempts++;
        item.Message = $"Claimed by worker (attempt {item.RetryAttempts}).";
        item.UpdatedAtUtc = DateTimeOffset.UtcNow;
        db.RequestEvents.Add(new RequestEvent { KycRequestId = item.Id, Status = item.Status, Message = item.Message });
        await db.SaveChangesAsync(ct);
        return item;
    }

    public async Task<KycRequest?> CompleteAsync(int id, bool success, string message, CancellationToken ct)
    {
        var item = await db.KycRequests.FindAsync([id], ct);
        if (item is null) return null;

        if (success)
        {
            item.Status = "Completed";
            item.Message = "Demo processing completed successfully.";
        }
        else if (item.RetryAttempts < item.MaxRetryAttempts)
        {
            item.Status = "Retry";
            item.Message = $"Simulated processing failure. Retry {item.RetryAttempts} of {item.MaxRetryAttempts}.";
        }
        else
        {
            item.Status = "Rejected";
            item.Message = "Demo request rejected after maximum retry attempts.";
        }

        item.UpdatedAtUtc = DateTimeOffset.UtcNow;
        db.RequestEvents.Add(new RequestEvent { KycRequestId = item.Id, Status = item.Status, Message = item.Message });
        await db.SaveChangesAsync(ct);
        return item;
    }
}
