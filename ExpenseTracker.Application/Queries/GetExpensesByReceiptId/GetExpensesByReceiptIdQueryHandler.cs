using ExpenseTracker.Application.DTO;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Interfaces;
using MediatR;

namespace ExpenseTracker.Application.Queries.GetExpensesByReceiptId
{
    public class GetExpensesByReceiptIdQueryHandler : IRequestHandler<GetExpensesByReceiptIdQuery, IReadOnlyList<ExpenseDto>>
    {
        private readonly IExpenseReader _expenseReader;
        private readonly ICurrentUserProvider _currentUserProvider;

        public GetExpensesByReceiptIdQueryHandler(IExpenseReader expenseReader, ICurrentUserProvider currentUserProvider)
        {
            _expenseReader = expenseReader;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<IReadOnlyList<ExpenseDto>> Handle(GetExpensesByReceiptIdQuery request, CancellationToken cancellationToken)
        {
            var currentUser = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);
            var expenses = await _expenseReader.GetByReceiptIdAsync(request.ReceiptId, currentUser.Id, cancellationToken);

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
                ReceiptId = expense.ReceiptId
            }).ToList();
        }
    }
}
