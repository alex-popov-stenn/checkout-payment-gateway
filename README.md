Assumptions:
1. Banking simulator API for payment creation is idempotent and handled by banking simulator due to lack of explicit fields in the API
2. Merchant will have integration with API Gateway via API
3. For the simplicy of implementation due to lack of persistent storage or messaging system, all payments are processed via synchronous API 
without network failures and concurrent requests
4. MerchantId passed a header which uniqely identifies the client (for simplicty)

Key design consideration:
1. Separaton of concerns
2. Simplicy / Maintainability
  -- Clean architecture (placing domain / architecte in the middle)
  -- Ubiqitious language
3. Fail fast
  -- Contract Guards
4. Payment Exactly once processing
-- Idempotency Key
5. External Reference Id
-- for reconcillation processes
6. External Metadata
-- for attaching analytics / additional data
7. Clean architecture for maintainability and domain focused application
8. Resource-based REST API Level 2 for simplicity
9. Testability
10. Open Closed

Production readiness action items:
1. Durability
2. Exactly once processing (consistency over availability)
3. Asynchronous processing of payments with resilience techniquies in the payment gateway to 
avoid data integrity issues due to potential concurrent requests and network failures between payment gateway and acquiring bank
4. Observability: metrics, logging, tracing
    -- key metrics
        API failure rate
        Latency
        count of payments (failed, succeeded)
        failed validations
        acquiring bank api (failure rate, latency percentiles)
        count of payments processing right now
5. Authentication and Authroization
6. Encyrption of sensetive card data in the database (encryption at rest)
7. Encryption in transit (Merchant Service -> API Gateway), (API Gateway -> Bank API)
8. Sensetive data masking in logging
9. Confirm with legal team requirements around GDRP, Data Privacy, PCI DSS, SCA, etc
10. Configure static code analysis to standartize code style and minimize code issues
11. MSBuild to keep package versions, and project settings in one place

Libraries used:
- Refit to generate a client to call external service (TOO COMPLICATED FOR THIS PROBLEM)
- fluent validation for validating requests
- OneOf for functional styles

- xunit + bogus, webapplication factory for tests, and test containers

Nice to have:
- configurability (currencies, amount limits, etc)

Risk areas
-- Validation
-- Exactly once processing

1. Components tests (start simulator with test containers testcointainers) (e2e)
    -- authorized (201) -> then get
    -- declined (201) -> then get
    -- rejected (400) -> then get 404
2. Unit tests (value objects validation)
    -- including expiry date corner cases (very close in the future)
    -- payment creation
3. 4 digits only (compiance)
4. Security check (404)

Value objects:
CardDetails
Money (Amount + Currency)