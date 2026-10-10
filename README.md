# eKYC Integration Platform — Portfolio Demo

An independent portfolio application demonstrating consent management, eKYC request submission, asynchronous background processing, retry handling, and operational visibility.

**This is a simulated demo.** It is not connected to any bank, identity provider, or production eKYC service. Never use real identity data.

## Stack
- .NET 8 ASP.NET Core Web API
- SQLite database via EF Core
- Hosted background worker in a separate project
- Static responsive dashboard (HTML/CSS/JavaScript)
- Swagger/OpenAPI

## Projects
- `src/eKYC.Api` — API, SQLite persistence, dashboard, worker-facing queue endpoints
- `src/eKYC.Worker` — background service that polls the API and simulates processing
- `docs/ARCHITECTURE.md` — request flow and design decisions
- `docs/PORTFOLIO_CASE_STUDY.md` — portfolio-ready project description

## Run in Visual Studio
1. Install the .NET 8 SDK and Visual Studio 2022.
2. Open `eKYCIntegrationPlatform.sln`.
3. Restore NuGet packages.
4. Set `eKYC.Api` as the startup project and run it (F5).
5. Open the dashboard at `/` or Swagger at `/swagger`.
6. To run the worker, start a second terminal from the solution folder:
   ```bash
   dotnet run --project src/eKYC.Worker
   ```
7. Submit a demo request in the dashboard. The worker will poll the API and update the request status.

The API creates `ekyc-demo.db` on first startup in its content root. Data is local to this demo.

## Demo endpoints
- `GET /api/health`
- `GET /api/consent`
- `POST /api/consent`
- `POST /api/consent/{id}/revoke`
- `GET /api/kyc/requests`
- `POST /api/kyc/requests`
- `GET /api/kyc/requests/{id}`
- `GET /api/worker/next` — claim the next pending/retry request
- `POST /api/worker/{id}/complete` — submit a simulated worker result

## Sample request
```json
{
  "customerReference": "DEMO-CUST-1001",
  "identityType": "NationalId",
  "identityNumber": "DEMO-ID-1001",
  "isLegacy": false,
  "requester": "Portfolio Demo"
}
```

## Demo behavior
- New requests begin in `Pending`.
- The worker claims pending or retryable requests and simulates a result.
- A request whose identity number contains `SIMULATE-FAIL` will fail once, retry, then finish as `Rejected` after the configured retry limit.
- All requests and status transitions are stored in SQLite.
- No real identity checks or consent notifications occur.

## Safety and production notes
This demo intentionally omits real identity verification, authentication, encryption/key management, consent/legal wording, and production data retention controls. Add these before any real deployment. Do not put real national identity numbers or customer information in this public demo.

## Portfolio description
Built an independent eKYC integration demo using .NET 8, ASP.NET Core Web API, SQLite, and a separate background worker. Implemented consent grant/revoke, request tracking, retryable asynchronous processing, status history, and a dashboard for operational visibility. The provider is simulated and no real identity data is processed.
