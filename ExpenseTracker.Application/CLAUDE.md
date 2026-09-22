# Backend (.NET 10, Clean Architecture)

Dependencies point inward: `ExpenseTrackerAPI` → `Application` → `Domain`. `Infrastructure` implements the Domain interfaces.

## Where things go
- `ExpenseTracker.Domain/`: entities (created with a static `Create(...)`), enums, value objects, exceptions (`DomainException`, `NotFoundException`, `ForbiddenException`), and the reader/writer interfaces (`IExpenseReader`, `IExpenseWriter`, …).
- `ExpenseTracker.Application/`: one folder per use case, `Commands/<Name>/` or `Queries/<Name>/`, containing `<Name>Command.cs` (a MediatR record), `<Name>CommandHandler.cs` and `<Name>Validator.cs` (FluentValidation, run automatically by `Behaviours/ValidationBehaviour`). DTOs go in `DTO/`.
- `ExpenseTracker.Infrastructure/`: EF Core (`Persistence/`), the repositories that implement Domain interfaces, auth, AI extraction and receipt storage. Register new services in its `DependencyInjection.cs`.
- `ExpenseTrackerAPI/Controllers/`: kept thin, each action just calls `_mediator.Send(...)`. Handlers throw domain exceptions; `Middleware/ExceptionHandlingMiddleware` maps them to 400/403/404 responses with a `{ "error" }` or `{ "errors": [...] }` body.
- `ExpenseTracker.Tests/`: mirrors the layers.

## Patterns to copy
- New endpoint: copy `Commands/AddExpense/` (command, handler, validator), add the controller action, then add tests like `Tests/Application/Commands/AddExpenseCommandHandlerTests.cs` (xUnit, Moq, FluentAssertions).
- Access control: handlers check membership with `IExpenseTableReader.IsMemberAsync` and throw `NotFoundException` for non-members.

## Commands (from repo root)
- `dotnet build`
- `dotnet test 2>&1 | tail -5`. The build prints many known CS8618 warnings, so only look at the full log when a test fails.
- Migrations: `dotnet ef migrations add <Name> --project ExpenseTracker.Infrastructure --startup-project ExpenseTrackerAPI`
- Run: `dotnet run --project ExpenseTrackerAPI --launch-profile ai`
