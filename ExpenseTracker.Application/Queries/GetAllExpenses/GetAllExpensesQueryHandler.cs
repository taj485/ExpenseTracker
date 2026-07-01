using ExpenseTracker.Application.DTO;
using ExpenseTracker.Application.Queries.GetExpenseById;
using ExpenseTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ExpenseTracker.Application.Queries.GetAllExpenses
{
    public class GetAllExpensesQueryHandler : IRequestHandler<GetAllExpensesQuery, IReadOnlyList<ExpenseDto>>
    {
        private readonly IExpenseReader _expenseReader;

        public GetAllExpensesQueryHandler(IExpenseReader expenseReader)
        {
            _expenseReader = expenseReader;
        }

        public async Task<IReadOnlyList<ExpenseDto>> Handle(GetAllExpensesQuery request, CancellationToken cancellationToken)
        {
            var expenses = await _expenseReader.GetAllAsync(cancellationToken);

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
