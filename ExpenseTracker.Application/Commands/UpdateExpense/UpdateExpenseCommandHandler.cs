using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;
using System.Linq;

namespace ExpenseTracker.Application.Commands.UpdateExpense
{
    public class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, Unit>
    {
        private readonly IExpenseReader _expenseReader;
        private readonly IExpenseWriter _expenseWriter;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IValidator<UpdateExpenseCommand> _validator;

        public UpdateExpenseCommandHandler(IExpenseReader expenseReader, IExpenseWriter expenseWriter, ICurrentUserProvider currentUserProvider, IValidator<UpdateExpenseCommand> validator)
        {
            _expenseReader = expenseReader;
            _expenseWriter = expenseWriter;
            _currentUserProvider = currentUserProvider;
            _validator = validator;
        }

        public async Task<Unit> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var expense = await _expenseReader.GetByIdAsync(request.Id, cancellationToken);

            if (expense is null)
                throw new NotFoundException($"Expense with id {request.Id} was not found");

            var currentUser = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);
            if (!expense.Users.Any(u => u.Id == currentUser.Id))
                throw new NotFoundException($"Expense with id {request.Id} was not found");

            expense.UpdateAmount(request.Amount);
            expense.UpdateCategory(request.Category);
            expense.UpdateDescription(request.Description);

            await _expenseWriter.UpdateAsync(expense);

            return Unit.Value;
        }
    }
}
