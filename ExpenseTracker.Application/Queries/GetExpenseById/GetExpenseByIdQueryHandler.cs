using ExpenseTracker.Application.DTO;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ExpenseTracker.Application.Queries.GetExpenseById
{
    public class GetExpenseByIdQueryHandler : IRequestHandler<GetExpenseByIdQuery, ExpenseDto>
    {
        private readonly IExpenseReader _expenseReader;
        private readonly IExpenseTableReader _expenseTableReader;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly GetExpenseByIdValidator _validator;

        public GetExpenseByIdQueryHandler(IExpenseReader expenseReader, IExpenseTableReader expenseTableReader, ICurrentUserProvider currentUserProvider)
        {
            _expenseReader = expenseReader;
            _expenseTableReader = expenseTableReader;
            _currentUserProvider = currentUserProvider;
            _validator = new GetExpenseByIdValidator();
        }

        public async Task<ExpenseDto> Handle(GetExpenseByIdQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var expense = await _expenseReader.GetByIdAsync(request.Id, cancellationToken);

            if (expense is null)
                throw new NotFoundException($"Expense with id {request.Id} was not found");

            var currentUser = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);
            if (!await _expenseTableReader.IsMemberAsync(expense.ExpenseTableId, currentUser.Id, cancellationToken))
                throw new NotFoundException($"Expense with id {request.Id} was not found");

            return new ExpenseDto
            {
                Id = expense.Id,
                UnitPrice = expense.UnitPrice.Amount,
                Quantity = expense.Quantity,
                Currency = expense.UnitPrice.Currency,
                Category = expense.Category.ToString(),
                Description = expense.Description,
                Date = expense.Date,
                MerchantId = expense.MerchantId,
                Merchant = expense.Merchant?.Name,
                MerchantWebsite = expense.Merchant?.Website,
                ReceiptId = expense.ReceiptId,
                ExpenseTableId = expense.ExpenseTableId
            };
        }
    }
}
