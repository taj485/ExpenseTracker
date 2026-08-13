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
    "BlobStorage__ConnectionString"        = azurerm_storage_account.receipts.primary_connection_string
    "BlobStorage__ContainerName"           = azurerm_storage_container.receipts.name
    "BlobStorage__TempContainerName"       = azurerm_storage_container.receipts_temp.name

    # Receipt extraction now runs in the separate receipt-analyser service. Until a broker and that
    # service are deployed these stay empty, and extraction will fail in production while the rest
    # of the app is unaffected.
    "Kafka__BootstrapServers"        = var.kafka_bootstrap_servers
    "ReceiptAnalyser__BaseUrl"       = var.receipt_analyser_base_url
    "ReceiptAnalyser__TokenEndpoint" = "https://${var.auth0_domain}/oauth/token"
    "ReceiptAnalyser__Audience"      = var.auth0_audience
    "ReceiptAnalyser__ClientId"      = var.receipt_analyser_client_id
    "ReceiptAnalyser__ClientSecret"  = var.receipt_analyser_client_secret
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

# Landing spot for a receipt photo between upload and the user deciding to keep it. On save the API
# server-side copies the blob into the permanent container under the same name; on abandon the
# lifecycle rule below is the entire cleanup story.
resource "azurerm_storage_container" "receipts_temp" {
  name                  = var.blob_temp_container_name
  storage_account_id    = azurerm_storage_account.receipts.id
  container_access_type = "private"
}

# Azure allows exactly one management policy per storage account (its ARM name is always "default"),
# so both rules must live in this single resource. Adding a second
# azurerm_storage_management_policy for the same account would silently fight with this one.
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

  rule {
    name    = "delete-temp-after-1-day"
    enabled = true

    filters {
      prefix_match = [azurerm_storage_container.receipts_temp.name]
      blob_types   = ["blockBlob"]
    }

    actions {
      base_blob {
        # Azure evaluates lifecycle rules roughly daily, so this is "within ~24-48h" in practice.
        # Never treat a temp blob as guaranteed gone by a deadline.
        delete_after_days_since_creation_greater_than = 1
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
