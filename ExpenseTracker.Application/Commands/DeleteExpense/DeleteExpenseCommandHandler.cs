using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;
using System.Linq;

namespace ExpenseTracker.Application.Commands.DeleteExpense
{
    public class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand, Unit>
    {
        private readonly IExpenseReader _expenseReader;
        private readonly IExpenseWriter _expenseWriter;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IValidator<DeleteExpenseCommand> _validator;

        public DeleteExpenseCommandHandler(IExpenseReader expenseReader, IExpenseWriter expenseWriter, ICurrentUserProvider currentUserProvider, IValidator<DeleteExpenseCommand> validator)
        {
            _expenseReader = expenseReader;
            _expenseWriter = expenseWriter;
            _currentUserProvider = currentUserProvider;
            _validator = validator;
        }

        public async Task<Unit> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
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

            await _expenseWriter.DeleteAsync(request.Id);

            return Unit.Value;
        }
    }
}
