using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace eKYC.Worker;

public sealed class KycProcessingWorker(
    IHttpClientFactory httpClientFactory,
    IOptions<WorkerOptions> options,
    ILogger<KycProcessingWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = httpClientFactory.CreateClient("eKYC.Api");
        var settings = options.Value;
        logger.LogInformation("eKYC demo worker started. API base URL: {BaseUrl}", client.BaseAddress);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var claimResponse = await client.GetAsync("api/worker/next", stoppingToken);
                if (claimResponse.StatusCode == HttpStatusCode.NoContent)
                {
                    await Task.Delay(TimeSpan.FromSeconds(Math.Max(1, settings.PollIntervalSeconds)), stoppingToken);
                    continue;
                }

                claimResponse.EnsureSuccessStatusCode();
                var request = await claimResponse.Content.ReadFromJsonAsync<ClaimedRequest>(cancellationToken: stoppingToken);
                if (request is null) continue;

                await Task.Delay(Math.Max(100, settings.SimulatedProcessingMilliseconds), stoppingToken);

                var success = !request.IdentityNumber.Contains("SIMULATE-FAIL", StringComparison.OrdinalIgnoreCase);
                using var completionResponse = await client.PostAsJsonAsync(
                    $"api/worker/{request.Id}/complete",
                    new { success, message = success ? "Simulated identity provider accepted request." : "Simulated provider timeout." },
                    stoppingToken);
                completionResponse.EnsureSuccessStatusCode();
                logger.LogInformation("Processed KYC request {RequestId}; success={Success}", request.Id, success);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Worker cycle failed. Check that eKYC.Api is running and the base URL matches its launch profile.");
                await Task.Delay(TimeSpan.FromSeconds(Math.Max(2, settings.PollIntervalSeconds)), stoppingToken);
            }
        }
    }

    private sealed record ClaimedRequest(int Id, string IdentityType, string IdentityNumber, int RetryAttempts);
}
