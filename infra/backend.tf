terraform {
  backend "azurerm" {
    resource_group_name  = "rg-expensetracker-tfstate"
    storage_account_name = "stexpensetrackertf01"
    container_name       = "tfstate"
    key                  = "expensetracker.terraform.tfstate"
  }
}
