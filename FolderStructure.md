ExpenseTracker/
├── ExpenseTracker.sln
├── docker-compose.yml
├── .dockerignore
├── CLAUDE.md
├── README.md
├── .github/
│   └── workflows/
│       ├── ci-cd.yml
│       └── self-host-deploy.yml
├── .claude/
│   └── memory.md
├── Doc/
│   └── azure-hosting-and-cicd.md
├── infra/
│   ├── terraform.tf
│   ├── providers.tf
│   ├── backend.tf
│   ├── variables.tf
│   ├── main.tf
│   ├── outputs.tf
│   └── terraform.tfvars.example
├── Client/
│   ├── mockups/
│   │   ├── mockup-1.html
│   │   ├── mockup-2.html
│   │   ├── mockup-3.html
│   │   ├── mockup-combined.html
│   │   ├── mockup-quirky.html
│   │   ├── mockup-professional.html
│   │   └── mockup-genz.html
│   ├── src/
│   │   ├── styles.css
│   │   ├── types/
│   │   │   └── heic2any.d.ts
│   │   ├── environments/
│   │   │   ├── environment.ts
│   │   │   └── environment.prod.ts
│   │   └── app/
│   │       ├── app.ts
│   │       ├── app.routes.ts
│   │       ├── app.config.ts
│   │       ├── core/
│   │       │   ├── models/
│   │       │   │   ├── expense.model.ts
│   │       │   │   └── expense-table.model.ts
│   │       │   ├── services/
│   │       │   │   ├── expense.service.ts
│   │       │   │   ├── expense.service.spec.ts
│   │       │   │   ├── expense-table.service.ts
│   │       │   │   ├── add-expense-drawer.service.ts
│   │       │   │   ├── upload-receipt-drawer.service.ts
│   │       │   │   ├── image-resize.service.ts
│   │       │   │   └── image-resize.service.spec.ts
│   │       │   ├── utils/
│   │       │   │   ├── category.utils.ts
│   │       │   │   ├── date.utils.ts
│   │       │   │   ├── date.utils.spec.ts
│   │       │   │   ├── download.utils.ts
│   │       │   │   ├── expense.utils.ts
│   │       │   │   ├── expense.utils.spec.ts
│   │       │   │   ├── heic-converter.ts
│   │       │   │   ├── heic-converter.spec.ts
│   │       │   │   ├── merchant.utils.ts
│   │       │   │   └── merchant.utils.spec.ts
│   │       │   └── auth/
│   │       │       ├── auth.guard.ts
│   │       │       ├── auth.guard.spec.ts
│   │       │       ├── auth.interceptor.ts
│   │       │       └── auth.interceptor.spec.ts
│   │       ├── layout/
│   │       │   ├── shell/
│   │       │   │   ├── shell.component.ts
│   │       │   │   ├── shell.component.html
│   │       │   │   └── shell.component.css
│   │       │   ├── sidebar/
│   │       │   │   ├── sidebar.component.ts
│   │       │   │   ├── sidebar.component.html
│   │       │   │   └── sidebar.component.css
│   │       │   └── topbar/
│   │       │       ├── topbar.component.ts
│   │       │       ├── topbar.component.html
│   │       │       └── topbar.component.css
│   │       ├── shared/
│   │       │   ├── drag-to-dismiss.directive.ts
│   │       │   ├── confirm-dialog/
│   │       │   │   ├── confirm-dialog.component.ts
│   │       │   │   ├── confirm-dialog.component.html
│   │       │   │   └── confirm-dialog.component.css
│   │       │   └── merchant-logo/
│   │       │       ├── merchant-logo.component.ts
│   │       │       ├── merchant-logo.component.html
│   │       │       └── merchant-logo.component.css
│   │       └── features/
│   │           ├── home/
│   │           │   ├── home.component.ts
│   │           │   ├── home.component.html
│   │           │   └── home.component.css
│   │           ├── dashboard/
│   │           │   ├── dashboard.component.ts
│   │           │   ├── dashboard.component.html
│   │           │   ├── dashboard.component.css
│   │           │   └── components/
│   │           │       ├── summary-cards/
│   │           │       │   ├── summary-cards.component.ts
│   │           │       │   ├── summary-cards.component.html
│   │           │       │   └── summary-cards.component.css
│   │           │       ├── category-breakdown/
│   │           │       │   ├── category-breakdown.component.ts
│   │           │       │   ├── category-breakdown.component.html
│   │           │       │   └── category-breakdown.component.css
│   │           │       └── merchant-donut/
│   │           │           ├── merchant-donut.component.ts
│   │           │           ├── merchant-donut.component.html
│   │           │           ├── merchant-donut.component.css
│   │           │           └── merchant-donut.component.spec.ts
│   │           ├── expenses/
│   │           │   ├── expense-list/
│   │           │   │   ├── expense-list.component.ts
│   │           │   │   ├── expense-list.component.html
│   │           │   │   └── expense-list.component.css
│   │           │   ├── expense-detail/
│   │           │   │   ├── expense-detail.component.ts
│   │           │   │   ├── expense-detail.component.html
│   │           │   │   └── expense-detail.component.css
│   │           │   ├── expense-edit/
│   │           │   │   ├── expense-edit.component.ts
│   │           │   │   ├── expense-edit.component.html
│   │           │   │   └── expense-edit.component.css
│   │           │   └── receipt-group/
│   │           │       ├── receipt-group.component.ts
│   │           │       ├── receipt-group.component.html
│   │           │       └── receipt-group.component.css
│   │           ├── add-expense/
│   │           │   ├── add-expense-form.component.ts
│   │           │   ├── add-expense-form.component.html
│   │           │   └── add-expense-form.component.css
│   │           ├── upload-receipt/
│   │           │   ├── upload-receipt.component.ts
│   │           │   ├── upload-receipt.component.html
│   │           │   └── upload-receipt.component.css
│   │           └── expense-table/
│   │               ├── create-expense-table-prompt.component.ts
│   │               ├── create-expense-table-prompt.component.html
│   │               └── create-expense-table-prompt.component.css
│   ├── public/
│   │   ├── favicon.ico
│   │   └── staticwebapp.config.json
│   ├── proxy.conf.json
│   ├── angular.json
│   └── package.json
│
├── ExpenseTracker.Domain/
│   ├── Entities/
│   │   ├── Expense.cs
│   │   ├── ExpenseTable.cs
│   │   ├── Merchant.cs
│   │   ├── MerchantAlias.cs
│   │   ├── Receipt.cs
│   │   ├── User.cs
│   │   └── UserExpenseTable.cs
│   ├── ValueObjects/
│   │   ├── Money.cs
│   │   ├── ExtractedReceiptItem.cs
│   │   └── ReceiptImage.cs
│   ├── Enums/
│   │   └── ExpenseCategory.cs
│   ├── Interfaces/
│   │   ├── IExpenseReader.cs
│   │   ├── IExpenseWriter.cs
│   │   ├── IExpenseTableReader.cs
│   │   ├── IExpenseTableWriter.cs
│   │   ├── IExpenseRepository.cs
│   │   ├── IMerchantReader.cs
│   │   ├── IMerchantWriter.cs
│   │   ├── IUserReader.cs
│   │   ├── IUserWriter.cs
│   │   ├── ICurrentUserService.cs
│   │   ├── IReceiptExtractionService.cs
│   │   ├── IReceiptWriter.cs
│   │   ├── IReceiptReader.cs
│   │   └── IReceiptImageStore.cs
│   ├── Services/
│   │   ├── ISummaryCalculator.cs
│   │   ├── MonthlySummaryCalculator.cs
│   │   └── WeeklySummaryCalculator.cs
│   ├── Exceptions/
│   │   ├── DomainException.cs
│   │   ├── ForbiddenException.cs
│   │   └── NotFoundException.cs
│   └── AssemblyInfo.cs
│
├── ExpenseTracker.Tests/
│   ├── Domain/
│   │   ├── MoneyTests.cs
│   │   ├── ExpenseTests.cs
│   │   ├── MerchantTests.cs
│   │   ├── ExpenseTableTests.cs
│   │   └── UserTests.cs
│   ├── Application/
│   │   ├── Commands/
│   │   │   ├── AddExpenseCommandHandlerTests.cs
│   │   │   ├── AddExpensesBatchCommandHandlerTests.cs
│   │   │   ├── UpdateExpenseCommandHandlerTests.cs
│   │   │   ├── DeleteExpenseCommandHandlerTests.cs
│   │   │   ├── ExtractReceiptExpensesCommandHandlerTests.cs
│   │   │   ├── CreateExpenseTableCommandHandlerTests.cs
│   │   │   ├── InviteUserToTableCommandHandlerTests.cs
│   │   │   ├── RemoveUserFromTableCommandHandlerTests.cs
│   │   │   ├── DeleteExpenseTableCommandHandlerTests.cs
│   │   │   ├── StarExpenseTableCommandHandlerTests.cs
│   │   │   ├── UnstarExpenseTableCommandHandlerTests.cs
│   │   │   └── UploadReceiptImageCommandHandlerTests.cs
│   │   ├── Queries/
│   │   │   ├── GetAllExpensesQueryHandlerTests.cs
│   │   │   ├── GetExpenseQueryHandlerTests.cs
│   │   │   ├── GetExpenseTablesForUserQueryHandlerTests.cs
│   │   │   └── GetReceiptImageQueryHandlerTests.cs
│   │   └── Services/
│   │       ├── CurrentUserProviderTests.cs
│   │       └── MerchantResolverTests.cs
│   ├── Infrastructure/
│   │   ├── ExpenseRepositoryTests.cs
│   │   ├── ExpenseTableRepositoryTests.cs
│   │   ├── MerchantRepositoryTests.cs
│   │   ├── MerchantSeedDataTests.cs
│   │   ├── UserRepositoryTests.cs
│   │   └── StorageProviderRegistrationTests.cs
│   └── Api/
│       └── ExpenseControllerAuthTests.cs
│
├── ExpenseTrackerAPI/
│   ├── Dockerfile
│   ├── Controllers/
│   │   ├── ExpenseController.cs
│   │   └── ExpenseTableController.cs
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs
│   ├── Program.cs
│   └── appsettings.json
│
├── ExpenseTracker.Application/
│   ├── Behaviours/
│   │   └── ValidationBehaviour.cs
│   ├── Commands/
│   │   ├── AddExpense/
│   │   │   ├── AddExpenseCommand.cs
│   │   │   ├── AddExpenseCommandHandler.cs
│   │   │   └── AddExpenseValidator.cs
│   │   ├── AddExpensesBatch/
│   │   │   ├── AddExpensesBatchCommand.cs
│   │   │   └── AddExpensesBatchCommandHandler.cs
│   │   ├── UpdateExpense/
│   │   │   ├── UpdateExpenseCommand.cs
│   │   │   ├── UpdateExpenseCommandHandler.cs
│   │   │   └── UpdateExpenseValidator.cs
│   │   ├── DeleteExpense/
│   │   │   ├── DeleteExpenseCommand.cs
│   │   │   ├── DeleteExpenseCommandHandler.cs
│   │   │   └── DeleteExpenseValidator.cs
│   │   ├── ExtractReceiptExpenses/
│   │   │   ├── ExtractReceiptExpensesCommand.cs
│   │   │   └── ExtractReceiptExpensesCommandHandler.cs
│   │   ├── CreateExpenseTable/
│   │   │   ├── CreateExpenseTableCommand.cs
│   │   │   ├── CreateExpenseTableCommandHandler.cs
│   │   │   └── CreateExpenseTableValidator.cs
│   │   ├── InviteUserToTable/
│   │   │   ├── InviteUserToTableCommand.cs
│   │   │   ├── InviteUserToTableCommandHandler.cs
│   │   │   └── InviteUserToTableValidator.cs
│   │   ├── RemoveUserFromTable/
│   │   │   ├── RemoveUserFromTableCommand.cs
│   │   │   ├── RemoveUserFromTableCommandHandler.cs
│   │   │   └── RemoveUserFromTableValidator.cs
│   │   ├── DeleteExpenseTable/
│   │   │   ├── DeleteExpenseTableCommand.cs
│   │   │   ├── DeleteExpenseTableCommandHandler.cs
│   │   │   └── DeleteExpenseTableValidator.cs
│   │   ├── StarExpenseTable/
│   │   │   ├── StarExpenseTableCommand.cs
│   │   │   ├── StarExpenseTableCommandHandler.cs
│   │   │   └── StarExpenseTableValidator.cs
│   │   ├── UnstarExpenseTable/
│   │   │   ├── UnstarExpenseTableCommand.cs
│   │   │   ├── UnstarExpenseTableCommandHandler.cs
│   │   │   └── UnstarExpenseTableValidator.cs
│   │   └── UploadReceiptImage/
│   │       ├── UploadReceiptImageCommand.cs
│   │       └── UploadReceiptImageCommandHandler.cs
│   ├── Queries/
│   │   ├── GetExpenseById/
│   │   │   ├── GetExpenseByIdQuery.cs
│   │   │   └── GetExpenseByIdQueryHandler.cs
│   │   ├── GetAllExpenses/
│   │   │   ├── GetAllExpensesQuery.cs
│   │   │   └── GetAllExpensesQueryHandler.cs
│   │   ├── GetExpensesByReceiptId/
│   │   │   ├── GetExpensesByReceiptIdQuery.cs
│   │   │   └── GetExpensesByReceiptIdQueryHandler.cs
│   │   ├── GetMonthlySummary/
│   │   │   ├── GetMonthlySummaryQuery.cs
│   │   │   └── GetMonthlySummaryQueryHandler.cs
│   │   ├── GetExpenseTablesForUser/
│   │   │   ├── GetExpenseTablesForUserQuery.cs
│   │   │   └── GetExpenseTablesForUserQueryHandler.cs
│   │   └── GetReceiptImage/
│   │       ├── GetReceiptImageQuery.cs
│   │       └── GetReceiptImageQueryHandler.cs
│   ├── DTO/
│   │   ├── ExpenseDto.cs
│   │   ├── ExpenseTableDto.cs
│   │   ├── MonthlySummaryDto.cs
│   │   ├── ExtractedExpenseDto.cs
│   │   └── ReceiptImageDto.cs
│   ├── Mappings/
│   │   └── ExpenseMappingProfile.cs
│   └── Services/
│       ├── ICurrentUserProvider.cs
│       ├── CurrentUserProvider.cs
│       ├── IMerchantResolver.cs
│       └── MerchantResolver.cs
│
└── ExpenseTracker.Infrastructure/
    ├── Persistence/
    │   ├── ExpenseTrackerDbContext.cs
    │   ├── MerchantSeedData.cs
    │   ├── Configurations/
    │   │   ├── ExpenseConfigurations.cs
    │   │   ├── ExpenseTableConfiguration.cs
    │   │   ├── MerchantConfiguration.cs
    │   │   ├── MerchantAliasConfiguration.cs
    │   │   ├── ReceiptConfigurations.cs
    │   │   ├── UserConfiguration.cs
    │   │   └── UserExpenseTableConfiguration.cs
    │   └── Repositories/
    │       ├── ExpenseRepository.cs
    │       ├── ExpenseTableRepository.cs
    │       ├── MerchantRepository.cs
    │       ├── ReceiptRepository.cs
    │       └── UserRepository.cs
    ├── Migrations/
    ├── Auth/
    │   ├── AuthenticationServiceCollectionExtensions.cs
    │   └── CurrentUserService.cs
    ├── AI/
    │   ├── GeminiOptions.cs
    │   └── GeminiReceiptExtractionService.cs
    ├── Storage/
    │   ├── AzureBlobStorageOptions.cs
    │   ├── AzureBlobReceiptImageStore.cs
    │   ├── MinioStorageOptions.cs
    │   └── MinioReceiptImageStore.cs
    └── DependencyInjection.cs