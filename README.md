# Timesheet System

A simplified timesheet management application built as a technical assessment.

## Tech Stack

- **Framework**: Blazor Web App (.NET 10)
- **Architecture**: Layered (Core / Infrastructure / Web / Tests)
- **Storage**: In-memory (no database)
- **Testing**: xUnit + Moq

## Getting Started

### Prerequisites
- .NET 10 SDK
- Visual Studio 2026 or later

### Running the App
1. Clone the repository
2. Open `TimesheetSystem/TimesheetSystem.slnx` in Visual Studio
3. Set `TimesheetSystem` as the startup project
4. Press F5 or click Run

### Running the Tests
1. Open Test Explorer via **View → Test Explorer**
2. Click **Run All**

## Project Structure
```
TimesheetSystem                # Blazor Web App (UI)
TimesheetSystem.Core           # Domain models, interfaces, business logic
TimesheetSystem.Infrastructure # In-memory repository, seed data
TimesheetSystem.Tests          # xUnit unit tests
```

## Design Decisions

- **Interfaces for repository and service layers** — keeps business logic testable in isolation via mocking, and decouples storage from logic
- **Singleton repository, scoped service** — the repository is a singleton so the in-memory list persists for the lifetime of the app; the service is scoped as is standard for Blazor Server
- **Seed data for users and projects** — rather than implementing user/project management (out of scope), a small static set of users and projects is defined upfront, keeping the focus on timesheet functionality
- **DateOnly for date fields** — cleaner than DateTime for a date-only concept, available since .NET 6
- **Duplicate entry prevention** — enforced at the service layer; the same user cannot log time to the same project on the same date twice

## Assumptions

- Users and projects are predefined — no registration or project creation is required
- The working week runs Monday to Sunday
- Entries cannot be logged for future dates
- Hours must be between 0 and 24 per entry