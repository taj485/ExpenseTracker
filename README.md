# ExpenseTracker

A full-stack expense tracking app — Angular SPA + ASP.NET Core (Clean Architecture) API, backed by PostgreSQL, with Auth0 authentication.

## Tech Stack

- **Frontend**: Angular (standalone components)
- **Backend**: ASP.NET Core Web API, EF Core, MediatR, Clean Architecture (Domain/Application/Infrastructure)
- **Database**: PostgreSQL
- **Auth**: Auth0 (JWT bearer)
- **Hosting**: Azure App Service + Azure Static Web Apps, provisioned via Terraform

## AI dev mode

Runs the app without an Auth0 login, for AI agents and quick local checks. Every request is treated as one fixed local user.

- API: `dotnet run --project ExpenseTrackerAPI --launch-profile ai`
- Client: `cd Client && npm run start:ai`

It's off unless you start it this way. The API refuses to start with `Auth0:DevBypass` set outside Development, and the client stand-in (`DevAuthService`) is only compiled into the `ai` build configuration.

## Documentation

- [Azure Hosting & CI/CD](Doc/azure-hosting-and-cicd.md) — infrastructure, deployment pipeline, and troubleshooting
