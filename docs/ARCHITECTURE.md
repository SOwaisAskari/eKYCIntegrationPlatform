# Architecture

```text
Dashboard / API Client
        |
        v
ASP.NET Core Web API
  | ConsentController
  | KycController
  | WorkerController
        |
        v
RequestService + EF Core
        |
        v
SQLite (consents, requests, status events)
        ^
        |
Separate .NET Background Worker
        |
        v
Simulated identity-provider outcome
```

## Main flow
1. A user grants `KycProcessing` consent for a fictional customer reference.
2. The user submits a request; the API rejects it if active consent is missing.
3. The request and initial status event are stored in SQLite.
4. The worker polls `GET /api/worker/next`, which claims the oldest pending/retry request.
5. The worker simulates a result and calls `POST /api/worker/{id}/complete`.
6. The API persists the result and status history.

## Important design boundaries
- The worker calls the API rather than accessing the database directly.
- Status changes are visible through the dashboard and API.
- Identity-number values are only demo strings; the worker does not perform verification.
- This simple sample is intended for local learning, not multi-worker production use. Production systems need authentication, atomic queue claiming, durable retries, privacy controls, and robust audit policy.
