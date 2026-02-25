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
