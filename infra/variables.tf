variable "resource_group_name" {
  description = "Name of the resource group that holds all app resources"
  type        = string
  default     = "rg-expensetracker-prod"
}

variable "location" {
  description = "Azure region for all resources"
  type        = string
  default     = "West Europe"
}

variable "app_service_plan_sku" {
  description = "SKU for the Linux App Service Plan"
  type        = string
  default     = "F1"
}

variable "api_app_name" {
  description = "Globally-unique name for the API App Service (becomes <name>.azurewebsites.net)"
  type        = string
}

variable "static_web_app_name" {
  description = "Name for the Azure Static Web App hosting the Angular SPA"
  type        = string
  default     = "swa-expensetracker-prod"
}

variable "auth0_domain" {
  description = "Auth0 tenant domain, e.g. dev-xxxx.us.auth0.com"
  type        = string
}

variable "auth0_audience" {
  description = "Auth0 API audience identifier"
  type        = string
}

variable "database_connection_string" {
  description = "Npgsql connection string for the production Postgres database"
  type        = string
  sensitive   = true
}
