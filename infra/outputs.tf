output "api_app_default_hostname" {
  description = "Default hostname of the API App Service"
  value       = azurerm_linux_web_app.api.default_hostname
}

output "static_web_app_default_hostname" {
  description = "Default hostname of the Static Web App"
  value       = azurerm_static_web_app.spa.default_host_name
}

output "static_web_app_api_key" {
  description = "Deployment token for the Static Web App (used by Azure/static-web-apps-deploy in CI)"
  value       = azurerm_static_web_app.spa.api_key
  sensitive   = true
}

output "storage_account_name" {
  description = "Name of the Storage Account holding receipt images"
  value       = azurerm_storage_account.receipts.name
}

output "blob_temp_container_name" {
  description = "Name of the container holding receipts awaiting a keep/abandon decision"
  value       = azurerm_storage_container.receipts_temp.name
}
