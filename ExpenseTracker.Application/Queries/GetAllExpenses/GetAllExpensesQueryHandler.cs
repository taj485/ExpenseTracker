using ExpenseTracker.Application.DTO;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Interfaces;
using MediatR;

namespace ExpenseTracker.Application.Queries.GetAllExpenses
{
    public class GetAllExpensesQueryHandler : IRequestHandler<GetAllExpensesQuery, IReadOnlyList<ExpenseDto>>
    {
        private readonly IExpenseReader _expenseReader;
        private readonly IExpenseTableReader _expenseTableReader;
        private readonly ICurrentUserProvider _currentUserProvider;

        public GetAllExpensesQueryHandler(IExpenseReader expenseReader, IExpenseTableReader expenseTableReader, ICurrentUserProvider currentUserProvider)
        {
            _expenseReader = expenseReader;
            _expenseTableReader = expenseTableReader;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<IReadOnlyList<ExpenseDto>> Handle(GetAllExpensesQuery request, CancellationToken cancellationToken)
        {
            var currentUser = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);

            var table = await _expenseTableReader.GetByIdAsync(request.ExpenseTableId, cancellationToken);
            if (table is null || !table.Members.Any(m => m.UserId == currentUser.Id))
                throw new NotFoundException($"Expense table with id {request.ExpenseTableId} was not found");

            var expenses = await _expenseReader.GetAllForTableAsync(request.ExpenseTableId, cancellationToken);

            return expenses
                .Select(expense => new ExpenseDto
            {
                Id = expense.Id,
                Amount = expense.Amount.Amount,
                Currency = expense.Amount.Currency,
                Category = expense.Category.ToString(),
                Description = expense.Description,
                Date = expense.Date,
                Merchant = expense.Merchant,
                ReceiptId = expense.ReceiptId,
                ExpenseTableId = expense.ExpenseTableId
            }).ToList();
        }
    }
}
