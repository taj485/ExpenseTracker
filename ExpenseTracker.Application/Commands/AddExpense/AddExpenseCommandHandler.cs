using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ExpenseTracker.Application.Commands.AddExpense
{
    public class AddExpenseCommandHandler : IRequestHandler<AddExpenseCommand, int>
    {
        private readonly IExpenseWriter _expenseWriter;
        private readonly IExpenseTableReader _expenseTableReader;
        private readonly IValidator<AddExpenseCommand> _validator;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IMerchantResolver _merchantResolver;

        public AddExpenseCommandHandler(IExpenseWriter expenseWriter, IExpenseTableReader expenseTableReader, IValidator<AddExpenseCommand> validator, ICurrentUserProvider currentUserProvider, IMerchantResolver merchantResolver)
        {
            _expenseWriter = expenseWriter;
            _expenseTableReader = expenseTableReader;
            _validator = validator;
            _currentUserProvider = currentUserProvider;
            _merchantResolver = merchantResolver;
        }

        public async Task<int> Handle(AddExpenseCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
              throw new ValidationException(validationResult.Errors);

            var currentUser = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);

            if (!await _expenseTableReader.IsMemberAsync(request.ExpenseTableId, currentUser.Id, cancellationToken))
                throw new NotFoundException($"Expense table with id {request.ExpenseTableId} was not found");

            var merchantId = await _merchantResolver.ResolveOrCreateAsync(request.Merchant, cancellationToken);

            var expense = Expense.Create(request.UnitPrice, request.Category, request.Description, request.Date, request.ExpenseTableId, merchantId, quantity: request.Quantity);
            int id = await _expenseWriter.AddAsync(expense, cancellationToken);
            return id;
        }
    }
}
