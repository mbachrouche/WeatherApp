# Weather Subscription API — AGENTS.md

## Project Overview

The **Weather Subscription API** is an ASP.NET Core backend service (net8.0) that manages weather subscriptions and fetches real-time weather data from OpenWeatherMap. Users subscribe by email to receive weather updates for their specified city. The system validates subscriptions, prevents duplicate emails, and integrates external weather APIs.

---

## Solution Structure

```
WeatherSubscription.sln
├── weather-subscription-api/          (Main API project)
└── weather-subscription-api.Tests/    (xUnit test project)
```

**Pinned to:** .NET 8.0.421 (via `global.json` with `rollForward: latestPatch`)

---

## Backend Folder Layout

```
weather-subscription-api/
├── appsettings.json                        DB & API keys
├── Program.cs                              Entry point (minimal stub)
├── Controllers/
│   └── SubscriptionsController.cs          POST /api/subscriptions
├── Domain/
│   ├── Entities/
│   │   └── Subscription.cs                 Entity model
│   └── Interfaces/
│       ├── ISubscriptionRepository.cs      Data access contract
│       └── IWeatherService.cs              Weather API contract
├── DTOs/
│   ├── Requests/
│   │   └── CreateSubscriptionRequest.cs    POST body schema
│   └── Responses/
│       ├── SubscriptionCreatedResponse.cs  POST 201 response
│       └── WeatherResponse.cs              Weather fields
├── Services/
│   ├── SubscriptionService.cs              Business logic
│   └── WeatherService.cs                   OpenWeatherMap integration
├── Infrastructure/
│   ├── Data/
│   │   ├── AppDbContext.cs                 EF DbContext
│   │   └── AppDbContextFactory.cs          DesignTimeFactory (for migrations)
│   ├── External/
│   │   └── OpenWeatherMapModels.cs         External API DTOs (placeholder)
│   └── Repositories/
│       └── SubscriptionRepository.cs       DB operations
├── Exceptions/
│   ├── DuplicateEmailException.cs          Duplicate email error
│   ├── NotFoundException.cs                 Subscription not found
│   └── WeatherApiException.cs              OpenWeatherMap API failure
└── Middleware/
    └── ExceptionHandlingMiddleware.cs      Error response standardization

test-subscription-api.Tests/
├── Services/
│   ├── SubscriptionServiceTests.cs         Subscription logic tests
│   └── WeatherServiceTests.cs              Weather service tests
├── Repositories/
│   └── SubscriptionRepositoryTests.cs      Repository tests
└── TestResults/
    └── test_results.trx                    xUnit output
```

---

## Architecture & Dependency Flow

```
SubscriptionsController
  ↓ (injects)
SubscriptionService
  ├→ ISubscriptionRepository (implement: SubscriptionRepository)
  │  └→ AppDbContext
  └→ IWeatherService (implement: WeatherService)
     └→ HttpClient (OpenWeatherMap)

ExceptionHandlingMiddleware
  └→ Catches & standardizes all exceptions
```

**Design Principles:**
- **Controller:** Minimal — route mapping only, no business logic
- **Service Layer:** Domain logic (validation, orchestration)
- **Repository Pattern:** Abstracted data access; swap implementations for testing
- **Exception Handling:** Centralized via middleware; custom exceptions for domain events
- **Dependency Injection:** Wired in `Program.cs`

---

## API Endpoints

### 1. POST /api/subscriptions

**Purpose:** Create a new weather subscription.

**Request Body:**
```json
{
  "email": "user@example.com",
  "city": "London"
}
```

**DTO:** [CreateSubscriptionRequest](weather-subscription-api/DTOs/Requests/CreateSubscriptionRequest.cs)
- `Email` (string, required)
- `City` (string, required)

**Success Response:** HTTP 201 Created
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "email": "user@example.com",
  "city": "London",
  "createdAt": "2026-06-01T10:30:00Z"
}
```

**DTO:** [SubscriptionCreatedResponse](weather-subscription-api/DTOs/Responses/SubscriptionCreatedResponse.cs)

**Error Responses:**

| HTTP | Scenario | Exception Type | Body |
|------|----------|----------------|------|
| 400 | Email already subscribed | `DuplicateEmailException` | `{ "error": "Email already subscribed for weather updates" }` |
| 400 | Invalid email format | `ArgumentException` | `{ "error": "Invalid email format" }` |
| 500 | DB insertion fails | `InvalidOperationException` | `{ "error": "Failed to create subscription" }` |

---

### 2. GET /api/subscriptions/{id}/weather

**Purpose:** Fetch live weather for a subscription's city.

**Route Parameter:**
- `id` (Guid) — subscription ID

**Success Response:** HTTP 200 OK
```json
{
  "city": "London",
  "description": "Partly Cloudy",
  "temperatureCelsius": 18.5,
  "cloudiness": "Partly Cloudy"
}
```

**DTO:** [WeatherResponse](weather-subscription-api/DTOs/Responses/WeatherResponse.cs)
- `City` (string)
- `Description` (string) — e.g., "Scattered clouds"
- `TemperatureCelsius` (decimal)

**Error Responses:**

| HTTP | Scenario | Exception Type | Body |
|------|----------|----------------|------|
| 404 | Subscription ID not found | `NotFoundException` | `{ "error": "Subscription not found" }` |
| 503 | OpenWeatherMap API down | `WeatherApiException` | `{ "error": "Weather service unavailable" }` |

---

## CloudCover Mapping

The API maps OpenWeatherMap's `clouds.all` field (0–100%) to human-readable cloudiness:

| Cloud % Range | Output |
|---------------|--------|
| 0–25% | Clear |
| 26–75% | Partly Cloudy |
| 76–100% | Overcast |

---

## Subscription Entity

**Namespace:** `WeatherSubscription.Api.Domain.Entities`  
**File:** [Subscription.cs](weather-subscription-api/Domain/Entities/Subscription.cs)

```csharp
public class Subscription
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

**Properties:**
- **Id** (Guid) — Primary key, auto-generated
- **Email** (string) — Email address; must be unique within the DB
- **City** (string) — Target city for weather subscription
- **CreatedAt** (DateTime) — UTC timestamp of subscription creation

**Validation:**
- Email: Non-null, valid format, unique in table
- City: Non-null, non-empty
- No constructor overloads; properties auto-initialize strings to `string.Empty`

---

## WeatherResponse Fields

**Namespace:** `WeatherSubscription.Api.DTOs.Responses`  
**File:** [WeatherResponse.cs](weather-subscription-api/DTOs/Responses/WeatherResponse.cs)

```csharp
public class WeatherResponse
{
    public string City { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal TemperatureCelsius { get; set; }
}
```

**Cloudiness Interpretation** (not yet in current entity — **implementation task**):
- **0–25%:** "Clear"
- **26–75%:** "Partly Cloudy"
- **76–100%:** "Overcast"

⚠️ **Contradiction Flagged:** `WeatherResponse` currently has only `City`, `Description`, and `TemperatureCelsius`. The `cloudiness` interpretation (above) is expected as per requirements but is **not yet modeled**. This will be added during Task 5 (API integration).

---

## Test Project Conventions

**Framework:** xUnit 2.4.2  
**Location:** [weather-subscription-api.Tests/](weather-subscription-api.Tests/)

**Test Dependencies (from .csproj):**
- **xunit** 2.4.2 — Test framework
- **xunit.runner.visualstudio** 2.4.5 — Test runner
- **Moq** 4.18.4 — Mocking library
- **FluentAssertions** 6.9.0 — Assertion fluency
- **Microsoft.EntityFrameworkCore.InMemory** 8.0.0 — In-memory DB (tests only)

**Convention Rules:**
- ✅ Always use **xUnit `[Fact]` or `[Theory]`** for test methods
- ✅ Mock `ISubscriptionRepository` and `IWeatherService` with **Moq 4.18.4**
- ✅ Use **FluentAssertions** for `result.Should().Be(...)` style assertions
- ✅ **EF InMemory only** for repository tests; never SQLite in tests
- ✅ Name test files as `*Tests.cs` (e.g., `SubscriptionServiceTests.cs`)
- ✅ One test class per service/repo class
- ✅ Arrange-Act-Assert (AAA) pattern

**Example Test Structure:**
```csharp
[Fact]
public async Task CreateSubscription_WithValidEmail_ReturnsId()
{
    // Arrange
    var mockRepo = new Mock<ISubscriptionRepository>();
    var service = new SubscriptionService(mockRepo.Object);
    var request = new CreateSubscriptionRequest { Email = "test@example.com", City = "London" };

    // Act
    var result = await service.CreateAsync(request);

    // Assert
    result.Should().NotBeEmpty();
}
```

---

## Frontend Layout

⚠️ **Not yet scoped.** Current repo contains backend only (ASP.NET Core API).  
Future frontend will likely include:
- Subscription form (email, city)
- Subscription list view
- Weather display per subscription

---

## Environment & Build Configuration

### Database

- **File:** `subscriptions.db` (SQLite)
- **Connection String:** `Data Source=subscriptions.db` (from [appsettings.json](weather-subscription-api/appsettings.json))
- **Location:** Relative to project root; created on first EF migration
- **Gitignored:** Yes (local dev artifact)

### SDK & Runtime

- **Pinned Version:** .NET 8.0.421
- **Configuration File:** [global.json](global.json)
  ```json
  {
    "sdk": {
      "version": "8.0.421",
      "rollForward": "latestPatch"
    }
  }
  ```
- **Effect:** All builds locked to 8.0.421; patch updates allowed (`rollForward: latestPatch`)

### Development Settings

- **File:** `appsettings.Development.json`
- **Status:** Gitignored (local dev only)
- **Recommended Contents:**
  ```json
  {
    "OpenWeatherMap": {
      "ApiKey": "YOUR_DEV_API_KEY"
    },
    "Logging": {
      "LogLevel": {
        "Default": "Debug"
      }
    }
  }
  ```
  (Never commit real API keys to appsettings.json)

### OpenWeatherMap API Configuration

- **BaseUrl:** `https://api.openweathermap.org/data/2.5/` (from appsettings.json)
- **ApiKey:** Injected from config; never hardcoded
- **Endpoint Used:** `weather?q={city}&appid={apiKey}&units=metric`

---

## Current Implementation Status

| Task | Description | Status |
|------|-------------|--------|
| 1 | Set up EF Core DbContext, migrations, and SQLite DB | ☐ |
| 2 | Implement `SubscriptionRepository` (Add, GetById, GetAll) | ☐ |
| 3 | Implement `SubscriptionService` (Create, Validate, Duplicate check) | ☐ |
| 4 | Wire DI & middleware in `Program.cs`; add Swagger | ☐ |
| 5 | Implement `WeatherService` (call OWM API, map clouds % to cloudiness) | ☐ |
| 6 | Implement `SubscriptionsController` (Create & GetWeather endpoints) | ☐ |
| 7 | Write all unit tests (xUnit + Moq + InMemory EF) | ☐ |
| 8 | End-to-end test (Postman/curl against live API) | ☐ |

---

## Development Rules & Constraints

### TDD Discipline
- **Always write failing test first** before implementing feature
- Red → Green → Refactor cycle mandatory
- Test must express the requirement; code satisfies it

### Code Organization
- **No business logic in controllers** — Controllers invoke services only
- **Services own validation and orchestration** — Repositories own data access
- **Exceptions over return codes** — Throw custom exceptions; middleware catches

### Database & Testing
- **EF InMemory in tests only** — Never SQLite for unit tests
- **Repository mock for service tests** — No actual DB access
- **One DB instance per test** — Isolated state; no test pollution

### Configuration Management
- **Never hardcode OWM API key** — Read from config/environment
- **Never commit secrets** — `.gitignore` covers `.db`, `appsettings.Development.json`
- **Use `appsettings.json` for non-sensitive defaults** — Overridden by environment-specific files

### Session & Task Management
- **One task per session** — Focus; avoid context switching
- **Run `dotnet test` at end of each task** — Verify all tests pass before moving on
- **Do not touch files outside current task scope** — Minimize diff noise; reduce merge conflicts
- **Commit between tasks** — Checkpoint each logical unit

---

## Contradictions Flagged

| Item | Expected | Found | Action |
|------|----------|-------|--------|
| WeatherResponse.Cloudiness | Model property for "Clear", "Partly Cloudy", "Overcast" mapping | Not present in current entity | Implement in Task 5 when adding OWM integration |
| Program.cs Integration | DI container, Swagger, EF migrations, middleware registration | Minimal stub (only root endpoint) | Implement in Task 4 (Program.cs wiring) |
| Test Implementations | Actual test methods with assertions | Stubs throwing `NotImplementedException` | Implement in Task 7 (Unit tests) |
| OpenWeatherMapModels | DTO classes for OWM API response deserialization | Placeholder class only | Implement in Task 5 (API integration) |

---

## Quick Commands

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run all unit tests (xUnit)
dotnet test

# Run API locally (http://localhost:5000)
dotnet run --project weather-subscription-api

# Create EF migration
dotnet ef migrations add InitialCreate --project weather-subscription-api

# Apply migration (creates subscriptions.db)
dotnet ef database update --project weather-subscription-api

# Clean
dotnet clean
```

---

## References

- **Entity Framework Core:** [Learn EF Core](https://docs.microsoft.com/en-us/ef/core/)
- **xUnit Testing:** [xUnit.net](https://xunit.net/)
- **Moq Documentation:** [Moq GitHub](https://github.com/Moq/moq4)
- **OpenWeatherMap API:** [API Docs](https://openweathermap.org/api)
- **ASP.NET Core Middleware:** [Custom Middleware](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware)
