using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ExpenseTracker.Application.Commands.StarExpenseTable
{
    public class StarExpenseTableCommandHandler : IRequestHandler<StarExpenseTableCommand, Unit>
    {
        private readonly IExpenseTableReader _expenseTableReader;
        private readonly IExpenseTableWriter _expenseTableWriter;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IValidator<StarExpenseTableCommand> _validator;

        public StarExpenseTableCommandHandler(IExpenseTableReader expenseTableReader, IExpenseTableWriter expenseTableWriter, ICurrentUserProvider currentUserProvider, IValidator<StarExpenseTableCommand> validator)
        {
            _expenseTableReader = expenseTableReader;
            _expenseTableWriter = expenseTableWriter;
            _currentUserProvider = currentUserProvider;
            _validator = validator;
        }

        public async Task<Unit> Handle(StarExpenseTableCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var currentUser = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);

            if (!await _expenseTableReader.IsMemberAsync(request.ExpenseTableId, currentUser.Id, cancellationToken))
                throw new NotFoundException($"Expense table with id {request.ExpenseTableId} was not found");

            await _expenseTableWriter.StarTableAsync(currentUser.Id, request.ExpenseTableId, cancellationToken);

            return Unit.Value;
        }
    }
}
