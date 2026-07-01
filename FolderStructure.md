ExpenseTracker/
├── ExpenseTracker.sln
├── CLAUDE.md
├── .claude/
│   └── memory.md
├── Client/
│   ├── mockups/
│   │   ├── mockup-1.html
│   │   ├── mockup-2.html
│   │   ├── mockup-3.html
│   │   └── mockup-combined.html
│   ├── src/
│   │   ├── styles.css
│   │   └── app/
│   │       ├── app.ts
│   │       ├── app.routes.ts
│   │       ├── app.config.ts
│   │       ├── core/
│   │       │   ├── models/
│   │       │   │   └── expense.model.ts
│   │       │   ├── services/
│   │       │   │   └── expense.service.ts
│   │       │   └── utils/
│   │       │       └── category.utils.ts
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
│   │       └── features/
│   │           ├── dashboard/
│   │           │   ├── dashboard.component.ts
│   │           │   ├── dashboard.component.html
│   │           │   ├── dashboard.component.css
│   │           │   └── components/
│   │           │       ├── summary-cards/
│   │           │       │   ├── summary-cards.component.ts
│   │           │       │   ├── summary-cards.component.html
│   │           │       │   └── summary-cards.component.css
│   │           │       └── category-breakdown/
│   │           │           ├── category-breakdown.component.ts
│   │           │           ├── category-breakdown.component.html
│   │           │           └── category-breakdown.component.css
│   │           ├── expenses/
│   │           │   ├── expense-list/
│   │           │   │   ├── expense-list.component.ts
│   │           │   │   ├── expense-list.component.html
│   │           │   │   └── expense-list.component.css
│   │           │   └── expense-detail/
│   │           │       ├── expense-detail.component.ts
│   │           │       ├── expense-detail.component.html
│   │           │       └── expense-detail.component.css
│   │           └── add-expense/
│   │               ├── add-expense-form.component.ts
│   │               ├── add-expense-form.component.html
│   │               └── add-expense-form.component.css
│   ├── proxy.conf.json
│   ├── angular.json
│   └── package.json
│
├── ExpenseTracker.Domain/
│   ├── Entities/
│   │   └── Expense.cs
│   ├── ValueObjects/
│   │   └── Money.cs
│   ├── Enums/
│   │   └── ExpenseCategory.cs
│   ├── Interfaces/
│   │   ├── IExpenseReader.cs
│   │   ├── IExpenseWriter.cs
│   │   └── IExpenseRepository.cs
│   ├── Services/
│   │   ├── ISummaryCalculator.cs
│   │   ├── MonthlySummaryCalculator.cs
│   │   └── WeeklySummaryCalculator.cs
│   └── Exceptions/
│       └── DomainException.cs
│
├── ExpenseTracker.Tests/
│   ├── Domain/
│   │   ├── MoneyTests.cs
│   │   └── ExpenseTests.cs
│   ├── Application/
│   │   └── Commands/
│   │       └── AddExpenseCommandHandlerTests.cs
│   └── Infrastructure/
│       └── Repositories/
│           └── ExpenseRepositoryTests.cs
│
├── ExpenseTrackerAPI/
│   ├── Controllers/
│   │   └── ExpenseController.cs
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
│   │   ├── UpdateExpense/
│   │   │   ├── UpdateExpenseCommand.cs
│   │   │   ├── UpdateExpenseCommandHandler.cs
│   │   │   └── UpdateExpenseValidator.cs
│   │   └── DeleteExpense/
│   │       ├── DeleteExpenseCommand.cs
│   │       └── DeleteExpenseCommandHandler.cs
│   ├── Queries/
│   │   ├── GetExpenseById/
│   │   │   ├── GetExpenseByIdQuery.cs
│   │   │   └── GetExpenseByIdQueryHandler.cs
│   │   ├── GetAllExpenses/
│   │   │   ├── GetAllExpensesQuery.cs
│   │   │   └── GetAllExpensesQueryHandler.cs
│   │   └── GetMonthlySummary/
│   │       ├── GetMonthlySummaryQuery.cs
│   │       └── GetMonthlySummaryQueryHandler.cs
│   ├── DTOs/
│   │   ├── ExpenseDto.cs
│   │   └── MonthlySummaryDto.cs
│   └── Mappings/
│       └── ExpenseMappingProfile.cs
│
└── ExpenseTracker.Infrastructure/
    ├── Persistence/
    │   ├── ExpenseTrackerDbContext.cs
    │   ├── Configurations/
    │   │   └── ExpenseConfiguration.cs
    │   ├── Migrations/
    │   └── Repositories/
    │       └── ExpenseRepository.cs
    └── DependencyInjection.cs