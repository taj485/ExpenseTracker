resource "azurerm_resource_group" "main" {
  name     = var.resource_group_name
  location = var.location
}

resource "azurerm_service_plan" "main" {
  name                = "asp-expensetracker-prod"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  os_type             = "Linux"
  sku_name            = var.app_service_plan_sku
}

resource "azurerm_linux_web_app" "api" {
  name                = var.api_app_name
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_service_plan.main.location
  service_plan_id     = azurerm_service_plan.main.id

  site_config {
    always_on = false # required: F1/Free tier does not support Always On

    application_stack {
      dotnet_version = "10.0"
    }
  }

  app_settings = {
    "ConnectionStrings__DefaultConnection" = var.database_connection_string
    "Auth0__Domain"                        = var.auth0_domain
    "Auth0__Audience"                      = var.auth0_audience
    "Cors__AllowedOrigin"                  = "https://${azurerm_static_web_app.spa.default_host_name}"
    "Gemini__ApiKey"                       = var.gemini_api_key
    "BlobStorage__ConnectionString"        = azurerm_storage_account.receipts.primary_connection_string
    "BlobStorage__ContainerName"           = azurerm_storage_container.receipts.name
  }
}

resource "azurerm_storage_account" "receipts" {
  name                     = var.storage_account_name
  resource_group_name      = azurerm_resource_group.main.name
  location                 = azurerm_resource_group.main.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  access_tier              = "Hot"
}

resource "azurerm_storage_container" "receipts" {
  name                  = var.blob_container_name
  storage_account_id    = azurerm_storage_account.receipts.id
  container_access_type = "private"
}

resource "azurerm_storage_management_policy" "receipts" {
  storage_account_id = azurerm_storage_account.receipts.id

  rule {
    name    = "move-to-cool-then-cold"
    enabled = true

    filters {
      prefix_match = [azurerm_storage_container.receipts.name]
      blob_types   = ["blockBlob"]
    }

    actions {
      base_blob {
        tier_to_cool_after_days_since_modification_greater_than = 20
        tier_to_cold_after_days_since_modification_greater_than = 40
      }
    }
  }
}

resource "azurerm_static_web_app" "spa" {
  name                = var.static_web_app_name
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku_tier            = "Free"
  sku_size            = "Free"

  # Deployment is handled by .github/workflows/ci-cd.yml via a deployment
  # token, not Azure's native repo-linked auto-deploy (which would also
  # require a repository_token here). Ignore the pre-existing repo link
  # so Terraform doesn't try to unlink it.
  lifecycle {
    ignore_changes = [repository_branch, repository_url]
  }
}
