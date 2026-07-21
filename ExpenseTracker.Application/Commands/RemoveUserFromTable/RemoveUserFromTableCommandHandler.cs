using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ExpenseTracker.Application.Commands.RemoveUserFromTable
{
    public class RemoveUserFromTableCommandHandler : IRequestHandler<RemoveUserFromTableCommand, Unit>
    {
        private readonly IExpenseTableReader _expenseTableReader;
        private readonly IExpenseTableWriter _expenseTableWriter;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IValidator<RemoveUserFromTableCommand> _validator;

        public RemoveUserFromTableCommandHandler(IExpenseTableReader expenseTableReader, IExpenseTableWriter expenseTableWriter, ICurrentUserProvider currentUserProvider, IValidator<RemoveUserFromTableCommand> validator)
        {
            _expenseTableReader = expenseTableReader;
            _expenseTableWriter = expenseTableWriter;
            _currentUserProvider = currentUserProvider;
            _validator = validator;
        }

        public async Task<Unit> Handle(RemoveUserFromTableCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var currentUser = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);

            if (!await _expenseTableReader.IsMemberAsync(request.ExpenseTableId, currentUser.Id, cancellationToken))
                throw new NotFoundException($"Expense table with id {request.ExpenseTableId} was not found");

            var isSelfRemoval = request.UserId == currentUser.Id;

            if (!isSelfRemoval && !await _expenseTableReader.IsAdminAsync(request.ExpenseTableId, currentUser.Id, cancellationToken))
                throw new ForbiddenException("Only an admin can remove another user from this expense table");

            var table = await _expenseTableReader.GetByIdAsync(request.ExpenseTableId, cancellationToken);

            if (table is null)
                throw new NotFoundException($"Expense table with id {request.ExpenseTableId} was not found");

            table.RemoveMember(request.UserId);
            await _expenseTableWriter.UpdateAsync(table);

            return Unit.Value;
        }
    }
}
