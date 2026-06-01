# AGENTS.md — Weather Subscription API

Read this file before doing anything in this repo.

-----

## Project Overview

A Weather Subscription API built as a Praktikum technical task.

Users create a subscription with email, city, and country, then retrieve live weather data for their saved location via OpenWeatherMap.

-----

## Solution Structure

```

WeatherSubscription.sln

├── weather-subscription-api/           ← ASP.NET Core Web API, .NET 8

├── weather-subscription-api.Tests/     ← xUnit test project

└── weather-subscription-frontend/      ← Vue 3 + Vite (bonus, Task 8)

```

**SDK pinned to:** .NET 8.0.421 via `global.json` (`rollForward: latestPatch`)

-----

## Backend Folder Layout

```

weather-subscription-api/

├── Controllers/

│   └── SubscriptionsController.cs

├── DTOs/

│   ├── Requests/

│   │   └── CreateSubscriptionRequest.cs

│   └── Responses/

│       ├── SubscriptionCreatedResponse.cs

│       └── WeatherResponse.cs

├── Domain/

│   ├── Entities/

│   │   └── Subscription.cs

│   └── Interfaces/

│       ├── ISubscriptionRepository.cs

│       └── IWeatherService.cs

├── Services/

│   ├── SubscriptionService.cs

│   └── WeatherService.cs

├── Infrastructure/

│   ├── Data/

│   │   ├── AppDbContext.cs

│   │   ├── AppDbContextFactory.cs

│   │   └── Migrations/               ← EF migrations live here (--output-dir was not set, so currently at weather-subscription-api/Migrations/)

│   ├── Repositories/

│   │   └── SubscriptionRepository.cs

│   └── External/

│       └── OpenWeatherMapModels.cs

├── Exceptions/

│   ├── DuplicateEmailException.cs

│   ├── NotFoundException.cs

│   └── WeatherApiException.cs

├── Middleware/

│   └── ExceptionHandlingMiddleware.cs

├── Program.cs

├── appsettings.json

└── appsettings.Development.json       ← gitignored, holds real OWM key

```

-----

## Architecture — Dependency Flow

```

HTTP Request

     ↓

Controller          → HTTP only, no business logic

     ↓

SubscriptionService → all business logic here

     ↓              ↘

Repository          WeatherService

     ↓                    ↓

  SQLite           OpenWeatherMap API

```

-----

## API Endpoints

### POST /subscriptions

**Request body:**

```json

{

  "email": "user@example.com",

  "city": "Berlin",

  "country": "DE",

  "zipCode": "10115"

}

```

`email`, `city`, `country` are required. `zipCode` is optional.

**Success — 201 Created:**

```json

{

  "id": 1,

  "email": "user@example.com",

  "city": "Berlin",

  "country": "DE",

  "zipCode": "10115",

  "createdAt": "2026-06-01T10:00:00Z"

}

```

### GET /subscriptions/{email}/weather

Lookup key is **email** (not an ID). Returns live weather for the saved location.

**Success — 200 OK:**

```json

{

  "city": "Berlin",

  "country": "DE",

  "description": "light rain",

  "temperature": {

    "current": 14.2,

    "min": 11.0,

    "max": 16.5

  },

  "pressure": 1012,

  "humidity": 78,

  "windSpeed": 5.3,

  "cloudiness": "Partly Cloudy",

  "sunrise": "05:42 AM",

  "sunset": "09:11 PM"

}

```

### Error Response Table

|Scenario                      |Status|

|------------------------------|------|

|Missing email / city / country|400   |

|Invalid email format          |400   |

|Duplicate email               |409   |

|Email not found               |404   |

|City not found on OWM         |404   |

|OpenWeatherMap unavailable    |503   |

|Any other exception           |500   |

**Error body shape:**

```json

{ "error": "A meaningful message here" }

```

-----

## Exception → HTTP Mapping

```

ArgumentException        → 400 Bad Request

DuplicateEmailException  → 409 Conflict

NotFoundException        → 404 Not Found

WeatherApiException      → 503 Service Unavailable

Any other exception      → 500 Internal Server Error

```

-----

## Subscription Entity

**Namespace:** `WeatherSubscriptionApi`

```csharp

public class Subscription

{

    public int Id { get; private set; }

    public string Email { get; private set; }

    public string City { get; private set; }

    public string Country { get; private set; }

    public string? ZipCode { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public Subscription(string email, string city, string country, string? zipCode = null)

    {

        // throws ArgumentException if email/city/country are null or whitespace

        Email = email.Trim().ToLowerInvariant();

        City = city.Trim();

        Country = country.Trim().ToUpperInvariant();

        ZipCode = zipCode?.Trim();

        CreatedAt = DateTime.UtcNow;

    }

    protected Subscription() { } // required by EF Core

}

```

Key facts:

- `Id` is `int`, not Guid

- Email is normalized to lowercase, Country to uppercase on construction

- Private setters — entity is only created via constructor

- Protected parameterless constructor exists for EF Core only

-----

## WeatherResponse DTO

All fields below must be present:

|Field      |Type   |Notes                                   |

|-----------|-------|----------------------------------------|

|city       |string |                                        |

|country    |string |                                        |

|description|string |e.g. “light rain”                       |

|temperature|object |current, min, max (all decimal, Celsius)|

|pressure   |int    |hPa                                     |

|humidity   |int    |percent                                 |

|windSpeed  |decimal|m/s                                     |

|cloudiness |string |mapped (see below)                      |

|sunrise    |string |formatted “hh:mm tt” e.g. “05:42 AM”    |

|sunset     |string |formatted “hh:mm tt” e.g. “09:11 PM”    |

**Cloudiness mapping** (from OWM `clouds.all` 0–100%):

|Range  |Output       |

|-------|-------------|

|0–25%  |Clear        |

|26–75% |Partly Cloudy|

|76–100%|Overcast     |

**Sunrise/sunset:** OWM returns Unix timestamps + timezone offset. Apply the offset before formatting.

-----

## Configuration

**appsettings.json:**

```json

{

  "OpenWeatherMap": {

    "ApiKey": "YOUR_API_KEY_HERE",

    "BaseUrl": "https://api.openweathermap.org/data/2.5"

  },

  "ConnectionStrings": {

    "DefaultConnection": "Data Source=subscriptions.db"

  }

}

```

Never hardcode the API key. Real key goes in `appsettings.Development.json` (gitignored).

-----

## Test Project

**Namespace:** `WeatherSubscription.Api.Tests`

**Packages (weather-subscription-api.Tests.csproj):**

- `xunit` 2.4.2

- `xunit.runner.visualstudio` 2.4.5

- `Microsoft.NET.Test.Sdk` 17.11.1

- `Moq` 4.20.72

- `Castle.Core` 5.1.1 (explicit reference — required for test host)

- `FluentAssertions` 6.9.0

- `Microsoft.EntityFrameworkCore.InMemory` 8.0.0

**Rules:**

- EF InMemory only — never real SQLite in tests

- Each repository test uses its own uniquely named InMemory DB (`Guid.NewGuid().ToString()`)

- Mock `ISubscriptionRepository` and `IWeatherService` with Moq in service tests

- Use FluentAssertions for all assertions

- AAA pattern (Arrange / Act / Assert)

-----

## Development Rules

1. **Always TDD** — write the failing test first, then implement to pass it

1. **No logic in controllers** — controllers call services and return results only

1. **EF InMemory in tests only** — never real SQLite

1. **Never hardcode the OWM API key** — always read from configuration

1. **One task per session** — do not implement multiple layers at once

1. **Run `dotnet test` at the end of every task** — report full output

1. **Do not touch files outside the current task scope**

1. **One focused commit per task** — use the commit message specified in the task prompt

-----

## Current Implementation Status

- [x] Task 1 — Domain entity (`Subscription.cs`) + interfaces — 5 tests passing

- [x] Task 2 — `AppDbContext`, `SubscriptionRepository`, EF migration — 6 tests passing (11 total)

- [ ] Task 3 — `WeatherService` + OWM mapping

- [ ] Task 4 — `SubscriptionService` orchestration

- [ ] Task 5 — Controller + DTOs

- [ ] Task 6 — `Program.cs` + DI wiring

- [ ] Task 7 — Middleware + error handling

- [ ] Task 8 — Frontend (Vue 3 + Vite)

-----

## Quick Commands

```bash

dotnet restore

dotnet build

dotnet test

dotnet run --project weather-subscription-api

# Migrations

dotnet ef migrations add <Name> --project weather-subscription-api --startup-project weather-subscription-api

dotnet ef database update --project weather-subscription-api --startup-project weather-subscription-api

```
 