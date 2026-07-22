using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ExpenseTracker.Application.Commands.CreateExpenseTable
{
    public class CreateExpenseTableCommandHandler : IRequestHandler<CreateExpenseTableCommand, int>
    {
        private readonly IExpenseTableReader _expenseTableReader;
        private readonly IExpenseTableWriter _expenseTableWriter;
        private readonly IValidator<CreateExpenseTableCommand> _validator;
        private readonly ICurrentUserProvider _currentUserProvider;

        public CreateExpenseTableCommandHandler(IExpenseTableReader expenseTableReader, IExpenseTableWriter expenseTableWriter, IValidator<CreateExpenseTableCommand> validator, ICurrentUserProvider currentUserProvider)
        {
            _expenseTableReader = expenseTableReader;
            _expenseTableWriter = expenseTableWriter;
            _validator = validator;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<int> Handle(CreateExpenseTableCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var currentUser = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);

            var existingTables = await _expenseTableReader.GetAllForUserAsync(currentUser.Id, cancellationToken);
            if (existingTables.Any(t => string.Equals(t.Name.Trim(), request.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
                throw new DomainException($"You already have a table named '{request.Name}'.");

            var expenseTable = ExpenseTable.Create(request.Name, currentUser.Id);
            int id = await _expenseTableWriter.AddAsync(expenseTable, cancellationToken);
            return id;
        }
    }
}
