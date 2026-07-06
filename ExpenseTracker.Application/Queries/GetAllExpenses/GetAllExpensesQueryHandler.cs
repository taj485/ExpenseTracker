using ExpenseTracker.Application.DTO;
using ExpenseTracker.Application.Queries.GetExpenseById;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ExpenseTracker.Application.Queries.GetAllExpenses
{
    public class GetAllExpensesQueryHandler : IRequestHandler<GetAllExpensesQuery, IReadOnlyList<ExpenseDto>>
    {
        private readonly IExpenseReader _expenseReader;
        private readonly ICurrentUserProvider _currentUserProvider;

        public GetAllExpensesQueryHandler(IExpenseReader expenseReader, ICurrentUserProvider currentUserProvider)
        {
            _expenseReader = expenseReader;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<IReadOnlyList<ExpenseDto>> Handle(GetAllExpensesQuery request, CancellationToken cancellationToken)
        {
            var currentUser = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);
            var expenses = await _expenseReader.GetAllForUserAsync(currentUser.Id, cancellationToken);

            return expenses
                .Select(expense => new ExpenseDto
            {
                Id = expense.Id,
                Amount = expense.Amount.Amount,
                Currency = expense.Amount.Currency,
                Category = expense.Category.ToString(),
                Description = expense.Description,
                Date = expense.Date
            }).ToList();
        }
    }
}
