# ExpenseTracker

A full-stack expense tracking app — Angular SPA + ASP.NET Core (Clean Architecture) API, backed by PostgreSQL, with Auth0 authentication.

## Tech Stack

- **Frontend**: Angular (standalone components)
- **Backend**: ASP.NET Core Web API, EF Core, MediatR, Clean Architecture (Domain/Application/Infrastructure)
- **Database**: PostgreSQL
- **Auth**: Auth0 (JWT bearer)
- **Hosting**: Azure App Service + Azure Static Web Apps, provisioned via Terraform

## Running locally

Pick one row and start both sides with the same mode. If the API and client modes don't match, logins and API calls won't line up.

| Goal | API (`dotnet run --project ExpenseTrackerAPI ...`) | Client (from `Client/`) | Login |
|---|---|---|---|
| Local dev without Auth0 | `--launch-profile ai` | `npm run start:ai` | Skipped (fixed dev user) |
| Local dev with real Auth0 | `--launch-profile http` | `npm start` | Real Auth0 |
| Mobile app on a phone | `--launch-profile mobile` | none (Expo app instead) | Real Auth0 |

The API always listens on port 5252, and the client proxies `/api` to it. The `https` profile is the same as `http` plus HTTPS on port 7122, which nothing in the repo uses.

## AI dev mode

Runs the app without an Auth0 login, for AI agents and quick local checks. Every request is treated as one fixed local user.

- API: `dotnet run --project ExpenseTrackerAPI --launch-profile ai`
- Client: `cd Client && npm run start:ai`

It's off unless you start it this way. The API refuses to start with `Auth0:DevBypass` set outside Development, and the client stand-in (`DevAuthService`) is only compiled into the `ai` build configuration.

## Mobile dev mode

Lets the mobile dev build on a phone reach your local API over Wi-Fi. Auth0 stays on.

- API: `dotnet run --project ExpenseTrackerAPI --launch-profile mobile` (listens on all interfaces, port 5252)
- Allow inbound TCP 5252 in Windows Firewall (private networks only)
- In the mobile repo's `.env.local`, set `EXPO_PUBLIC_API_URL=http://<your-PC-LAN-IP>:5252/api`

## Documentation

- [Azure Hosting & CI/CD](Doc/azure-hosting-and-cicd.md) — infrastructure, deployment pipeline, and troubleshooting
