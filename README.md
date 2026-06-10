# Weather Subscription API

A backend REST API built with ASP.NET Core (.NET 8) as part of a Praktikum technical task.

Users can create a weather subscription for their location and retrieve live weather data via OpenWeatherMap.

---

## Tech Stack

- **Backend:** ASP.NET Core Web API, .NET 8
- **Database:** SQLite via EF Core 8
- **External API:** OpenWeatherMap (free tier)
- **Testing:** xUnit, Moq, FluentAssertions, EF Core InMemory
- **Frontend (bonus):** Vue 3 + Vite

---

## Endpoints

|Method|Route                           |Description                              |

|------|--------------------------------|-----------------------------------------|

|POST  |`/subscriptions`                |Create a new subscription                |

|GET   |`/subscriptions/{email}/weather`|Get live weather for a saved subscription|

---

## Setup & Run

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- An [OpenWeatherMap API key](https://openweathermap.org/api) (free tier)

### Steps

```bash

# Clone the repo

git clone https://github.com/mbachrouche/WeatherApp.git

cd WeatherApp

# Add your API key

# Create weather-subscription-api/appsettings.Development.json with:

# {

#   "OpenWeatherMap": {

#     "ApiKey": "YOUR_API_KEY_HERE"

#   }

# }

# Restore and run

dotnet restore

dotnet run --project weather-subscription-api

```

API runs at `http://localhost:5000`.

### Run Tests

```bash

dotnet test

```

---

## Notes

- Email is used as the unique subscription key — no authentication required.
- OWM API key is never committed — stored in `appsettings.Development.json` (gitignored).
- All tests use EF Core InMemory — no real database needed for testing.
