# PowerPlant Coding Challenge

## Overview

This project is an ASP.NET Core Web API implementation of the PowerPlant Coding Challenge.

The API exposes one endpoint:

    POST /productionplan

It receives a payload containing:

- the requested load
- fuel prices
- wind percentage
- available power plants

It returns the amount of power each plant should produce in order to match the requested load while respecting the production constraints.

## Technologies

- C#
- .NET 10
- ASP.NET Core Web API
- xUnit
- Docker

## How to run locally

From the repository root:

    dotnet restore
    dotnet build
    dotnet test
    dotnet run --project src/PowerPlantCodingChallenge.Api

The API runs on:

    http://localhost:8888

Swagger UI is available at:

    http://localhost:8888/swagger

## Running with Docker

Build the Docker image:

    docker build -t powerplant-coding-challenge .

Run the container:

    docker run --rm -p 8888:8888 powerplant-coding-challenge

The API will be available at:

    http://localhost:8888

Swagger UI will be available at:

    http://localhost:8888/swagger

## API

### POST /productionplan

Example request:

    {
      "load": 910,
      "fuels": {
        "gas(euro/MWh)": 13.4,
        "kerosine(euro/MWh)": 50.8,
        "co2(euro/ton)": 20,
        "wind(%)": 60
      },
      "powerplants": [
        {
          "name": "gasfiredbig1",
          "type": "gasfired",
          "efficiency": 0.53,
          "pmin": 100,
          "pmax": 460
        },
        {
          "name": "gasfiredbig2",
          "type": "gasfired",
          "efficiency": 0.53,
          "pmin": 100,
          "pmax": 460
        },
        {
          "name": "gasfiredsomewhatsmaller",
          "type": "gasfired",
          "efficiency": 0.37,
          "pmin": 40,
          "pmax": 210
        },
        {
          "name": "tj1",
          "type": "turbojet",
          "efficiency": 0.3,
          "pmin": 0,
          "pmax": 16
        },
        {
          "name": "windpark1",
          "type": "windturbine",
          "efficiency": 1,
          "pmin": 0,
          "pmax": 150
        },
        {
          "name": "windpark2",
          "type": "windturbine",
          "efficiency": 1,
          "pmin": 0,
          "pmax": 36
        }
      ]
    }

Example response:

    [
      {
        "name": "windpark1",
        "p": 90.0
      },
      {
        "name": "windpark2",
        "p": 21.6
      },
      {
        "name": "gasfiredbig1",
        "p": 460.0
      },
      {
        "name": "gasfiredbig2",
        "p": 338.4
      },
      {
        "name": "gasfiredsomewhatsmaller",
        "p": 0.0
      },
      {
        "name": "tj1",
        "p": 0.0
      }
    ]

## Algorithm

The production plan is calculated using a merit-order dispatch strategy.

The main idea is to produce energy using the cheapest available power plants first.

The cost per MWh is calculated as follows:

### Wind turbine

    cost = 0

Wind turbines have no fuel cost. Their maximum available production is adjusted according to the wind percentage:

    available pmax = pmax * wind percentage / 100

### Gas-fired power plant

    cost = gas price / efficiency + co2 price * 0.3

The CO2 cost is included as a bonus feature. The challenge states that gas-fired power plants emit 0.3 tons of CO2 per MWh.

### Turbojet

    cost = kerosine price / efficiency

After calculating costs, power plants are sorted by merit order. The planner then dispatches production while respecting:

- `pmin`
- `pmax`
- wind availability
- requested load
- production precision of `0.1 MW`

The planner also handles cases where the remaining load is smaller than the next plant's `pmin`.

For example, if the requested load is `480 MW` and the first gas plant can produce `460 MW`, the remaining load is `20 MW`. If the next gas plant has a `pmin` of `100 MW`, it cannot simply produce `20 MW`. In that case, the planner redistributes production:

    gasfiredbig1 = 380
    gasfiredbig2 = 100
    total = 480

This keeps the result valid while avoiding the use of more expensive plants when a cheaper valid combination exists.

## Architecture

The solution is organized into separate projects:

    src/
      PowerPlantCodingChallenge.Api
      PowerPlantCodingChallenge.Application

    tests/
      PowerPlantCodingChallenge.UnitTests
      PowerPlantCodingChallenge.FunctionalTests

### API layer

The API layer contains:

- controllers
- request DTOs
- response DTOs
- dependency injection configuration

The controller is intentionally thin. It receives the HTTP request, maps it to application input, calls the application service, and returns the response.

### Application layer

The application layer contains the business use case and planning logic.

Main components:

- `IProductionPlanService`
- `ProductionPlanService`
- `IProductionPlanner`
- `MeritOrderProductionPlanner`
- `IProductionCostCalculator`
- `ProductionCostCalculator`

The planner is separated from the cost calculator to keep responsibilities clear.

### Domain layer

The domain project is kept available for domain concepts and future extension. For this challenge, most of the logic is contained in the application layer because the problem is mainly a calculation use case and does not require persistence.

## SOLID principles

The implementation applies SOLID principles pragmatically.

### Single Responsibility Principle

Each class has a focused responsibility.

- The controller handles HTTP concerns.
- The application service coordinates the use case.
- The planner handles production dispatch.
- The cost calculator handles cost calculation.

### Open/Closed Principle

The planner is hidden behind the `IProductionPlanner` interface. A different planning strategy could be added later without changing the API layer.

### Interface Segregation Principle

Interfaces are small and focused. For example, `IProductionCostCalculator` only exposes cost calculation behavior.

### Dependency Inversion Principle

The service depends on abstractions such as `IProductionPlanner`, not on concrete implementations. Dependencies are injected through ASP.NET Core dependency injection.

## Design patterns

### Strategy Pattern

`IProductionPlanner` represents a planning strategy.

The current implementation uses `MeritOrderProductionPlanner`, but another algorithm could be introduced later without changing the controller.

### Dependency Injection

ASP.NET Core dependency injection is used to wire services and planners.

This improves testability and keeps the application loosely coupled.

### DTO pattern

The API request and response models are separated from application models. This keeps the external API contract independent from the internal implementation.

## Testing

The solution contains both unit tests and functional tests.

Run all tests with:

    dotnet test

### Unit tests

Unit tests cover:

- production cost calculation
- wind availability
- merit-order dispatch
- payload1
- payload2
- payload3
- `pmin` redistribution behavior
- production values rounded to `0.1 MW`
- total production equal to requested load

### Functional tests

Functional tests cover the full HTTP pipeline using `WebApplicationFactory`.

They validate:

- `POST /productionplan` returns `200 OK` for valid payloads
- the API returns the expected production plan for the official example
- invalid input returns `400 Bad Request`

## Assumptions and limitations

This solution does not use an external linear programming solver, as requested by the challenge.

The implemented algorithm is a deterministic merit-order dispatch with additional handling for common `pmin` redistribution cases.

It is not intended to replace a full industrial unit commitment optimizer. The goal is to provide a clear, maintainable and testable solution adapted to the scope of the coding challenge.

## Bonus features implemented

- Docker support
- CO2 cost included for gas-fired power plants
- Swagger UI for quick manual testing