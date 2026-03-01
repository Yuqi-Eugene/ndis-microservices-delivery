# ndis-microservices-delivery

NDIS-inspired delivery workflow demo: Participants/Providers → Bookings → Service Deliveries → Approval → Claims.

## Tech Stack

- .NET Web API (Controllers)
- PostgreSQL
- Docker + docker compose
- GitHub Actions CI/CD
- AWS ECR + App Runner

## Layered Structure

``
src/Api/
  Domain/
    Entities/
    Enums/
  Application/
    Abstractions/
      Persistence/
    ServiceDeliveries/
      Commands/
      Queries/
      Handlers/
  Infrastructure/
    Persistence/
      AppDbContext.cs
      Repositories/
  Controllers/
  Auth/
  Program.cs
``

**Meaning**:
- Domain: pure business objects (no EF, no HTTP)
- Application: use cases (Commands/Queries + Handlers), depends on abstractions
- Infrastructure: EF Core, repositories
- Controllers: keep thin, just HTTP mapping

## Smoke Test The Running API

If the API is already running locally, you can run a full happy-path workflow without Swagger:

```bash
./scripts/smoke-api.sh
```

The script will:

- register and log in a new Provider user
- log in as the seeded Admin user
- create a Participant, Provider, and Booking
- confirm the Booking
- create, submit, and approve a Service Delivery
- create a Claim

If you are running on a different URL, pass it in:

```bash
./scripts/smoke-api.sh https://localhost:7199
```

The script uses these defaults, which you can override with environment variables:

- `ADMIN_EMAIL=admin@ndis.local`
- `ADMIN_PASSWORD=Admin123$`
- `PROVIDER_PASSWORD=Provider123$`
