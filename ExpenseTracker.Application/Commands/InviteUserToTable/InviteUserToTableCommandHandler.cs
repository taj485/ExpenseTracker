using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ExpenseTracker.Application.Commands.InviteUserToTable
{
    public class InviteUserToTableCommandHandler : IRequestHandler<InviteUserToTableCommand, Unit>
    {
        private readonly IExpenseTableReader _expenseTableReader;
        private readonly IExpenseTableWriter _expenseTableWriter;
        private readonly IUserReader _userReader;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IValidator<InviteUserToTableCommand> _validator;

        public InviteUserToTableCommandHandler(IExpenseTableReader expenseTableReader, IExpenseTableWriter expenseTableWriter, IUserReader userReader, ICurrentUserProvider currentUserProvider, IValidator<InviteUserToTableCommand> validator)
        {
            _expenseTableReader = expenseTableReader;
            _expenseTableWriter = expenseTableWriter;
            _userReader = userReader;
            _currentUserProvider = currentUserProvider;
            _validator = validator;
        }

        public async Task<Unit> Handle(InviteUserToTableCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var normalizedEmail = request.InviteeEmail.Trim().ToLowerInvariant();

            var currentUser = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);

            if (!await _expenseTableReader.IsMemberAsync(request.ExpenseTableId, currentUser.Id, cancellationToken))
                throw new NotFoundException($"Expense table with id {request.ExpenseTableId} was not found");

            if (!await _expenseTableReader.IsAdminAsync(request.ExpenseTableId, currentUser.Id, cancellationToken))
                throw new ForbiddenException("Only an admin can invite users to this expense table");

            var matches = await _userReader.GetAllByEmailAsync(normalizedEmail, cancellationToken);

            if (matches.Count == 0)
                throw new NotFoundException($"No user found with email {normalizedEmail}");

            if (matches.Count > 1)
                throw new DomainException($"Multiple users found with email {normalizedEmail}; cannot determine invitee");

            var table = await _expenseTableReader.GetByIdAsync(request.ExpenseTableId, cancellationToken);

            if (table is null)
                throw new NotFoundException($"Expense table with id {request.ExpenseTableId} was not found");

            table.AddMember(matches[0].Id, request.IsAdmin);
            await _expenseTableWriter.UpdateAsync(table);

            return Unit.Value;
        }
    }
}
