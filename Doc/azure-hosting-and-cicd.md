# Azure Hosting & CI/CD

How ExpenseTracker is hosted on Azure, how the infrastructure is provisioned, and how code gets deployed.

## Architecture

```mermaid
flowchart TB
    subgraph GH["GitHub — ci-cd.yml"]
        Push["Push / PR to main"]
        Backend["backend job\nbuild, test, publish"]
        Frontend["frontend job\nbuild, test"]
        DeployAPI["deploy-api job\n(needs: backend, frontend)"]
        DeploySPA["deploy-spa job\n(needs: backend, frontend)"]
        Push --> Backend
        Push --> Frontend
        Backend -- "api-publish artifact" --> DeployAPI
        Frontend -- "spa-dist artifact" --> DeploySPA
    end

    subgraph Azure["Azure (westeurope)"]
        RG["Resource Group\nrg-expensetracker-prod"]
        Plan["App Service Plan\nasp-expensetracker-prod (F1, Linux)"]
        API["App Service\napp-expensetracker-prod\n(.NET 10 API)"]
        SWA["Static Web App\nswa-expensetracker-prod\n(Angular SPA)"]
        RG --> Plan --> API
        RG --> SWA
    end

    DB[("Postgres\n(external, e.g. Supabase)")]
    Auth0["Auth0\n(JWT issuer)"]

    DeployAPI -- "OIDC login + zip deploy" --> API
    DeploySPA -- "SWA deploy token" --> SWA
    API --> DB
    API -.JWT validation.-> Auth0
    SWA -.login redirect.-> Auth0
    SWA -- "HTTPS calls" --> API
```

- **Terraform** provisions the Azure resources (resource group, plan, App Service, Static Web App) — it does not touch application code.
- **`ci-cd.yml`** builds, tests, and deploys application code into that already-provisioned infrastructure. It does not create or modify Azure resources.
- **Postgres** and **Auth0** are both external to Azure — provisioned/configured manually, referenced by the app via connection string / app settings.

## Terraform (`infra/`)

| File | Purpose |
|---|---|
| `terraform.tf` | Required Terraform version + provider version pin |
| `providers.tf` | `azurerm` provider block |
| `backend.tf` | Remote state config (Azure Storage) |
| `variables.tf` | All inputs — region, SKU, app names, Auth0 domain/audience, DB connection string |
| `main.tf` | The actual resources (resource group, service plan, API web app, static web app) |
| `outputs.tf` | Hostnames + the SWA deployment token (sensitive) |
| `terraform.tfvars.example` | Committed template — copy to `terraform.tfvars` (gitignored) and fill in real values |

### Remote state

State lives in Azure Blob Storage, not locally — so it isn't lost if you run Terraform from a different machine, and it supports locking to prevent concurrent applies. The storage account itself is bootstrapped **outside** Terraform (a Terraform backend can't manage the very storage account it depends on):

```bash
az group create --name rg-expensetracker-tfstate --location eastus
az storage account create --name <globally-unique-name> --resource-group rg-expensetracker-tfstate --sku Standard_LRS --encryption-services blob --min-tls-version TLS1_2
az storage container create --name tfstate --account-name <globally-unique-name>
```

Then fill in that storage account name in `backend.tf`.

### Running Terraform locally

```bash
cd infra
terraform init
terraform plan     # review carefully
terraform apply
```

Secrets (like the Postgres connection string) are supplied via `terraform.tfvars` (gitignored) or `TF_VAR_*` environment variables — never hardcoded into committed `.tf` files. `database_connection_string` is marked `sensitive = true` so it never prints in plan/apply output.

## CI/CD (`.github/workflows/ci-cd.yml`)

A single workflow with four jobs:

- **`backend`** / **`frontend`** — run on every push/PR to `main`. Build, test, and (on success) publish/build the deployable output, uploaded as a build artifact (`api-publish` / `spa-dist`).
- **`deploy-api`** / **`deploy-spa`** — `needs: [backend, frontend]`, so they only start if both test jobs succeed, and download the artifacts already built rather than rebuilding. Skipped entirely on `pull_request` runs (`if: github.event_name != 'pull_request'`) — only a push to `main` deploys.

This means a failing test blocks deployment outright (the deploy jobs never start), and the exact code that was tested is what gets deployed — no risk of the deploy step building something slightly different.

One simplification worth knowing: both `deploy-api` and `deploy-spa` run together on every push to `main`, even if only one side actually changed (e.g. an API-only fix still redeploys the unchanged SPA). It's harmless — redeploying identical static content is a no-op in practice — just not the most efficient. Path-based filtering (e.g. `dorny/paths-filter`) would make each deploy job conditional on its own directory changing, if that becomes worth the added complexity later.

### Authentication: OIDC, not stored secrets

The API deploy job authenticates to Azure via **federated credentials** (OIDC) instead of a stored client secret:

```bash
az ad app federated-credential create --id <sp-app-id> --parameters '{
  "name": "github-main",
  "issuer": "https://token.actions.githubusercontent.com",
  "subject": "repo:taj485/ExpenseTracker:ref:refs/heads/main",
  "audiences": ["api://AzureADTokenExchange"]
}'
```

GitHub Actions then exchanges a short-lived OIDC token for an Azure access token at runtime — no long-lived secret sitting in GitHub.

### Required GitHub secrets/variables

| Type | Name | Where it comes from |
|---|---|---|
| Secret | `AZURE_CLIENT_ID` | The Service Principal's App ID |
| Secret | `AZURE_TENANT_ID` | Azure AD tenant ID |
| Secret | `AZURE_SUBSCRIPTION_ID` | Target subscription ID |
| Secret | `AZURE_STATIC_WEB_APPS_API_TOKEN` | `terraform output -raw static_web_app_api_key` |
| Variable | `API_APP_NAME` | The App Service name (not secret — it's part of the public hostname anyway) |

## Azure App Service (the API)

- **Plan**: F1 (Free) tier, Linux. Free tier has real constraints: no custom Docker containers (Basic/B1+ required for those), and `always_on` must be `false` (the app idles after inactivity and cold-starts on the next request).
- **Deploy method**: native `.NET` zip-deploy (`dotnet publish` → `azure/webapps-deploy`), not a container — chosen specifically because F1 doesn't support containers.
- **Config**: Auth0 domain/audience, the Postgres connection string, and the allowed CORS origin are set as App Service **Application Settings** (`Auth0__Domain`, `Auth0__Audience`, `ConnectionStrings__DefaultConnection`, `Cors__AllowedOrigin` — double underscore maps to `:` in .NET config), managed by Terraform, not hardcoded in `appsettings.json`. `Cors__AllowedOrigin` is computed directly from the Static Web App resource's hostname, so it can't drift out of sync.
- **CORS**: the API and SPA are on different Azure domains, so the API explicitly allows the SPA's origin via `AddCors`/`UseCors` in `Program.cs` — without it the browser blocks the SPA's requests even though the URLs are otherwise correct.

## Azure Static Web Apps (the SPA)

- **Tier**: Free.
- **Deploy method**: `Azure/static-web-apps-deploy@v1`, building the Angular app on the GitHub Actions runner and uploading the pre-built output (`Client/dist/expense-tracker/browser`) with `skip_app_build: true`.
