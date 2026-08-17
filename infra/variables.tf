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

# Still consumed by the deployed API, which extracts receipts in-process. The Kafka-based
# receipt-analyser will take this over, but it is not deployed by this configuration yet, so
# removing this setting from the web app breaks receipt extraction in production.
variable "gemini_api_key" {
  description = "API key for the Google Gemini API used for receipt extraction"
  type        = string
  sensitive   = true
}

variable "receipt_analyser_base_url" {
  description = "Base URL of the receipt-analyser service the API reads extraction results from"
  type        = string
  default     = ""
}

variable "receipt_analyser_client_id" {
  description = "Auth0 machine-to-machine client id the API uses to call the receipt analyser"
  type        = string
  default     = ""
}

variable "receipt_analyser_client_secret" {
  description = "Auth0 machine-to-machine client secret the API uses to call the receipt analyser"
  type        = string
  default     = ""
  sensitive   = true
}

variable "kafka_bootstrap_servers" {
  description = "Kafka bootstrap servers the API publishes extraction requests to"
  type        = string
  default     = ""
}

variable "storage_account_name" {
  description = "Globally-unique name for the Storage Account holding receipt images (lowercase letters/numbers only, max 24 chars)"
  type        = string
}

variable "blob_container_name" {
  description = "Blob container name for storing receipt images"
  type        = string
  default     = "receipt-images"
}

# Deliberately not prefixed with blob_container_name's value: lifecycle prefix_match is a plain
# string prefix over "container/blob", so a name like "receipt-images-temp" would also be caught by
# the permanent container's tiering rule. See the delete rule in main.tf.
variable "blob_temp_container_name" {
  description = "Blob container holding freshly uploaded receipts awaiting a keep/abandon decision. Auto-deleted after a day."
  type        = string
  default     = "receipt-temp"
}
