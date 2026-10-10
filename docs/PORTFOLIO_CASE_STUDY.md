# eKYC Integration Platform — Case Study

**Objective:** Demonstrate an API-led, asynchronous eKYC workflow without publishing proprietary employer code.

**Implemented:** Consent grant/revoke, validation, request submission, status history, SQLite persistence, worker polling, retry handling, operational dashboard, and Swagger.

**Architecture choice:** The background worker communicates through API endpoints, keeping the processing worker decoupled from persistence details.

**Scope boundary:** Provider behavior is simulated. The sample does not verify real identities or store real customer data.
