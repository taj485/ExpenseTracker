using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ExpenseTracker.Application.Commands.DeleteExpenseTable
{
    public class DeleteExpenseTableCommandHandler : IRequestHandler<DeleteExpenseTableCommand, Unit>
    {
        private readonly IExpenseTableReader _expenseTableReader;
        private readonly IExpenseTableWriter _expenseTableWriter;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IValidator<DeleteExpenseTableCommand> _validator;

        public DeleteExpenseTableCommandHandler(IExpenseTableReader expenseTableReader, IExpenseTableWriter expenseTableWriter, ICurrentUserProvider currentUserProvider, IValidator<DeleteExpenseTableCommand> validator)
        {
            _expenseTableReader = expenseTableReader;
            _expenseTableWriter = expenseTableWriter;
            _currentUserProvider = currentUserProvider;
            _validator = validator;
        }

        public async Task<Unit> Handle(DeleteExpenseTableCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var currentUser = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);

            if (!await _expenseTableReader.IsMemberAsync(request.ExpenseTableId, currentUser.Id, cancellationToken))
                throw new NotFoundException($"Expense table with id {request.ExpenseTableId} was not found");

            if (!await _expenseTableReader.IsAdminAsync(request.ExpenseTableId, currentUser.Id, cancellationToken))
                throw new ForbiddenException("Only an admin can delete this expense table");

            await _expenseTableWriter.DeleteAsync(request.ExpenseTableId);

            return Unit.Value;
        }
    }
}
