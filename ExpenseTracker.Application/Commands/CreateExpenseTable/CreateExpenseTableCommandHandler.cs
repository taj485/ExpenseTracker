using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ExpenseTracker.Application.Commands.CreateExpenseTable
{
    public class CreateExpenseTableCommandHandler : IRequestHandler<CreateExpenseTableCommand, int>
    {
        private readonly IExpenseTableWriter _expenseTableWriter;
        private readonly IValidator<CreateExpenseTableCommand> _validator;
        private readonly ICurrentUserProvider _currentUserProvider;

        public CreateExpenseTableCommandHandler(IExpenseTableWriter expenseTableWriter, IValidator<CreateExpenseTableCommand> validator, ICurrentUserProvider currentUserProvider)
        {
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
            var expenseTable = ExpenseTable.Create(request.Name, currentUser.Id);
            int id = await _expenseTableWriter.AddAsync(expenseTable, cancellationToken);
            return id;
        }
    }
}
