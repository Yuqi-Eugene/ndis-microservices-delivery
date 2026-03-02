# NDIS Delivery API

This repository is a practical `.NET 8` backend sample built to demonstrate how a real business workflow can be modeled in a clean Web API. The domain is NDIS-inspired, but the learning value is broader: it shows how to build a CRUD-heavy business API with validation, authentication, role-based authorization, database persistence, and a predictable request pipeline.

If you are preparing for a `.NET backend` role, treat this project as a study lab. Do not just run it. Read it as if you are preparing to explain it in an interview:

- How does the HTTP request enter the system?
- Where is validation enforced?
- Where are business rules enforced?
- How is database consistency protected?
- How are authentication and authorization separated?
- Why are DTOs different from entities?

This README is written as a tutorial, not just a setup note.

## 1. What This Project Does

The API models a simple operational workflow:

1. Create `Participants` and `Providers`.
2. Create a `Booking` that links a participant to a provider.
3. Confirm the booking.
4. Create a `Service Delivery` against that confirmed booking.
5. Submit the service delivery for review.
6. Approve or reject the delivery.
7. Create a `Claim` only after the delivery has been approved.

That sequence is important because the code intentionally encodes business state transitions. This is exactly the kind of thing interviewers want backend engineers to understand: the API is not only storing data, it is protecting rules.

## 2. Why This Project Is Useful For Job Seekers

This repo helps you practice explaining several backend concepts that regularly appear in interviews:

- `ASP.NET Core Web API`: controllers, routing, model binding, middleware.
- `Entity Framework Core`: DbContext, entity mapping, relationships, indexes, migrations.
- `MediatR`: separating controllers from use-case handlers.
- `FluentValidation`: validating commands and queries before business logic runs.
- `JWT Authentication`: issuing tokens and reading claims from authenticated users.
- `ASP.NET Core Identity`: user storage, passwords, roles.
- `AutoMapper`: translating entities into DTOs returned by the API.
- `PostgreSQL`: relational persistence using `Npgsql`.
- `Swagger/OpenAPI`: interactive API exploration in development.

If you can confidently walk through this codebase, you can explain the difference between transport concerns, validation concerns, business concerns, and persistence concerns. That is a strong signal in backend interviews.

## 3. Tech Stack

- `.NET 8`
- `ASP.NET Core`
- `Entity Framework Core`
- `PostgreSQL 16`
- `ASP.NET Core Identity`
- `JWT Bearer Authentication`
- `MediatR`
- `FluentValidation`
- `AutoMapper`
- `Swagger / Swashbuckle`
- `Docker Compose`

## 4. Project Structure

The main application lives under `src/Api`.

```text
NdisDelivery/
  src/
    Api/
      Application/
        Auth/
        Bookings/
        Claims/
        Common/
        Participants/
        Providers/
        ServiceDeliveries/
      Auth/
      Controllers/
      Data/
      Domain/
      Dtos/
      Middlewares/
      Migrations/
      Program.cs
  scripts/
  docker-compose.yml
```

### How to Read the Structure

#### `Domain`

This is where the core business objects live:

- `Participant`
- `Provider`
- `Booking`
- `ServiceDelivery`
- `Claim`

These classes represent the shape of your business data. They are not HTTP-specific. They are also not UI-specific.

#### `Data`

This contains `AppDbContext`, the EF Core unit that:

- exposes tables through `DbSet<T>`
- configures relationships
- applies field constraints
- defines indexes

In interviews, this is where you explain how database schema rules are kept close to code.

#### `Dtos`

DTOs (Data Transfer Objects) are the contract between the API and clients.

Why not return entities directly?

- Entities are persistence models.
- DTOs are API models.
- DTOs let you change database internals without breaking clients.
- DTOs let you hide fields you do not want to expose.

This separation is a common interview topic and a core maintainability practice.

#### `Application`

This is the use-case layer. Each feature is organized into:

- `Commands`: requests that change state
- `Queries`: requests that read state
- `Handlers`: the actual business logic
- `Validators`: request validation rules

This is effectively a lightweight `CQRS` style.

The important idea: controllers do not contain business logic. They forward the request to MediatR, and handlers execute the use case.

#### `Controllers`

Controllers are deliberately thin.

They should mostly:

- accept HTTP input
- call MediatR
- return HTTP responses

This is the right design direction for backend systems that need to stay testable and maintainable.

#### `Middlewares`

The custom `ExceptionHandlingMiddleware` converts thrown exceptions into consistent HTTP error responses using `ProblemDetails`.

This teaches a good production habit: centralize cross-cutting concerns instead of repeating `try/catch` in every controller.

## 5. The Business Workflow in Plain English

The workflow is intentionally state-based:

- A booking starts in `Draft`.
- It can be moved to `Confirmed`.
- A service delivery can only be created if the booking is already `Confirmed`.
- A service delivery starts in `Draft`.
- It can be submitted.
- An admin can approve or reject it.
- A claim can only be created if the service delivery is `Approved`.
- A delivery can only have one claim.

This project is useful because it demonstrates rule enforcement across multiple entities. In interviews, that is often more valuable than simple CRUD.

## 6. How a Request Moves Through the System

A strong backend engineer should be able to narrate the request path clearly. In this project, the request flow looks like this:

1. An HTTP request hits a controller.
2. ASP.NET Core model binding turns JSON into a DTO or action parameters.
3. The controller sends a command/query to `MediatR`.
4. The `ValidationBehavior` runs all `FluentValidation` validators for that request type.
5. If validation passes, the target handler runs.
6. The handler loads data from `AppDbContext`, checks business rules, and changes state if needed.
7. The handler saves changes with EF Core.
8. AutoMapper converts the entity to a response DTO.
9. The controller returns the response.
10. If anything fails, `ExceptionHandlingMiddleware` maps exceptions into structured HTTP errors.

That flow is a good interview answer because it shows you understand the full request lifecycle, not only controller code.

## 7. Authentication and Authorization

The project uses two related but separate concepts:

### Authentication

Authentication answers: "Who is this user?"

This project uses:

- `ASP.NET Core Identity` to store users and passwords
- `JWT` to issue bearer tokens after login

The login endpoint returns a token. That token is sent in the `Authorization` header:

```http
Authorization: Bearer <jwt-token>
```

### Authorization

Authorization answers: "What is this user allowed to do?"

This project uses role-based authorization:

- `Provider`
- `Admin`

Examples:

- Providers can create and submit their own service deliveries.
- Admins can approve or reject service deliveries.

This distinction matters in interviews. Many candidates blur authentication and authorization. Do not.

## 8. Local Setup

### Prerequisites

Install:

- `.NET SDK 8`
- `Docker` with `docker compose`

Optional but useful:

- `dotnet-ef` CLI tool

If you do not have `dotnet-ef`, install it:

```bash
dotnet tool install --global dotnet-ef
```

### Step 1: Start PostgreSQL

From the repo root:

```bash
./scripts/db-up.sh
```

This starts a PostgreSQL 16 container using the configuration in `docker-compose.yml`.

Default database settings:

- Host: `localhost`
- Port: `5432`
- Database: `ndisdb`
- Username: `app`
- Password: `app`

### Step 2: Apply Migrations

```bash
./scripts/migrate.sh InitOrUpdate
```

What this script does:

- resolves the repo root
- points `dotnet ef` at `src/Api/Api.csproj`
- adds a migration with the name you provide
- updates the database

If migrations already exist and you only want to update the database, `dotnet ef database update` is enough, but the script is convenient for local iteration.

### Step 3: Run the API

```bash
./scripts/run-api.sh
```

This runs:

```bash
dotnet run --project src/Api
```

### Step 4: Open Swagger (Development Only)

When the API is running in Development, Swagger is enabled.

You can then explore endpoints, inspect request/response schemas, and manually test the workflow.

## 9. Important Configuration

The development settings live in `src/Api/appsettings.Development.json`.

Key values:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=ndisdb;Username=app;Password=app"
  },
  "Jwt": {
    "Key": "THIS_IS_A_DEV_ONLY_SECRET_KEY_CHANGE_ME",
    "Issuer": "ndis-api",
    "Audience": "ndis-api"
  },
  "Seed": {
    "AdminEmail": "admin@ndis.local",
    "AdminPassword": "Admin123$"
  }
}
```

### What These Settings Mean

- `ConnectionStrings:Default`: tells EF Core how to connect to PostgreSQL.
- `Jwt:*`: configures token signing and validation.
- `Seed:*`: creates a development admin user automatically at startup.

In production, these values should come from secure configuration sources such as environment variables or a secret manager, not hardcoded files.

## 10. The Default Seeded User

In Development, the application seeds:

- Role: `Admin`
- Role: `Provider`
- Default admin user:
  - Email: `admin@ndis.local`
  - Password: `Admin123$`

This is helpful for local testing because you can immediately log in as an admin and approve deliveries.

From a job-seeker perspective, this is also a good example of environment-specific bootstrapping logic.

## 11. Core Endpoints to Study

You do not need to memorize every endpoint. Focus on how the resources connect.

### Auth

- `POST /api/auth/register`
- `POST /api/auth/login`

Study this when you want to understand:

- how a provider user is created
- how JWT tokens are issued

### Participants

- `GET /api/participants`
- `GET /api/participants/{id}`
- `POST /api/participants`
- `PUT /api/participants/{id}`
- `DELETE /api/participants/{id}`

### Providers

- `GET /api/providers`
- `GET /api/providers/{id}`
- `POST /api/providers`
- `PUT /api/providers/{id}`
- `DELETE /api/providers/{id}`

### Bookings

- `GET /api/bookings`
- `GET /api/bookings/{id}`
- `POST /api/bookings`
- `PUT /api/bookings/{id}`
- `POST /api/bookings/{id}/confirm`
- `POST /api/bookings/{id}/cancel`

### Service Deliveries

- `GET /api/servicedeliveries`
- `GET /api/servicedeliveries/{id}`
- `POST /api/servicedeliveries`
- `PUT /api/servicedeliveries/{id}`
- `DELETE /api/servicedeliveries/{id}`
- `POST /api/servicedeliveries/{id}/submit`
- `POST /api/servicedeliveries/{id}/approve`
- `POST /api/servicedeliveries/{id}/reject`

### Claims

- `GET /api/claims`
- `POST /api/claims`

## 12. Fastest Way to Prove the System Works

Use the smoke test script:

```bash
./scripts/smoke-api.sh
```

If the API is running on another URL:

```bash
./scripts/smoke-api.sh https://localhost:7199
```

The smoke test performs the happy path:

1. Checks the participant endpoint is reachable.
2. Registers a new provider user.
3. Logs in as that provider.
4. Logs in as the seeded admin.
5. Creates a participant.
6. Creates a provider record.
7. Creates a booking.
8. Confirms the booking.
9. Creates a service delivery as the provider.
10. Submits the delivery as the provider.
11. Approves the delivery as the admin.
12. Creates a claim.

That script is an excellent study tool because it mirrors the exact business process.

## 13. What to Study First If You Are Learning

Read the code in this order:

### First: `Program.cs`

This file teaches you how the application is composed:

- dependency injection registration
- database registration
- authentication and authorization setup
- MediatR registration
- FluentValidation pipeline registration
- Swagger setup
- middleware order
- seeding roles and admin user

If you cannot explain `Program.cs`, you do not yet understand the application startup pipeline.

### Second: `Controllers`

Study one controller end to end, especially `ServiceDeliveriesController`.

Ask yourself:

- How does it extract the current user ID?
- How does it know whether the user is an admin?
- Why does it pass user context into commands?

This teaches API boundary design.

### Third: `ValidationBehavior`

This file explains a very important backend technique:

- requests are validated before handlers run
- validation becomes consistent and reusable
- controllers stay simple

This is a strong architectural pattern to discuss in interviews.

### Fourth: `CreateServiceDeliveryHandler`

This is one of the best examples of business rule enforcement in the codebase.

It shows:

- loading required related data
- rejecting invalid state transitions
- creating new entities only when rules are satisfied

### Fifth: `ExceptionHandlingMiddleware`

This teaches centralized error handling and standardized problem responses.

It is an example of reducing duplication while improving API consistency.

## 14. Interview Talking Points You Can Practice With This Repo

Use this project to practice answering questions like:

### "Why use MediatR here?"

Because it keeps controllers thin and pushes business logic into explicit use-case handlers. That makes the code easier to test, easier to reason about, and easier to extend without bloating controllers.

### "Why use DTOs instead of returning EF entities directly?"

Because DTOs protect the API contract from internal persistence changes, avoid overexposing fields, and make response shapes intentional.

### "Where should validation live?"

Basic request validation belongs in validators, not scattered manually through controllers. Business rule validation still belongs in handlers because it often depends on database state.

### "What is the difference between validation and business rules?"

- Validation checks whether the request shape and values are acceptable.
- Business rules check whether the requested action is allowed in the current domain state.

Example from this project:

- Validation: `Amount` must be positive.
- Business rule: a claim can only be created for an approved delivery.

### "Why use middleware for exception handling?"

Because it centralizes error translation, keeps controllers clean, and ensures clients receive consistent error payloads.

### "What problem does role-based authorization solve?"

It ensures only the correct user types can access sensitive actions, such as allowing only admins to approve service deliveries.

## 15. Good Backend Design Practices Demonstrated Here

This repo contains several habits worth copying:

- Thin controllers.
- Business logic in handlers.
- Validation before execution.
- Centralized exception handling.
- Explicit role checks.
- DTO boundaries.
- Relational integrity through foreign keys.
- Indexes on common lookup columns.
- Environment-specific developer conveniences such as Swagger and seeded data.

## 16. Limitations and Improvement Ideas

For learning purposes, this sample is intentionally simple. In a production system, you would likely add:

- richer automated tests
- structured logging with correlation IDs
- pagination on more endpoints
- audit trails
- optimistic concurrency handling
- domain events or outbox processing
- stricter authorization policies by ownership
- better secrets management
- production-grade deployment configuration

Knowing how to identify these gaps is also valuable in interviews.

## 17. Suggested Self-Study Exercises

If you want to get stronger as a job seeker, extend the codebase yourself:

1. Add unit tests for one handler.
2. Add integration tests for one full workflow.
3. Add pagination to bookings and claims.
4. Add a `Paid` transition for claims.
5. Add ownership checks so providers can only modify their own records.
6. Replace string statuses with enums plus value converters, then discuss tradeoffs.
7. Add request logging middleware and compare it with exception middleware.

Each exercise gives you something concrete to talk about in interviews.

## 18. Final Learning Advice

Do not study this project as a collection of files. Study it as a system.

When you read a feature, always connect these layers:

1. Controller
2. Command or Query
3. Validator
4. Handler
5. Entity and DbContext mapping
6. Response DTO

If you can explain one feature end to end in that sequence, you are thinking like a backend engineer instead of only a framework user.
