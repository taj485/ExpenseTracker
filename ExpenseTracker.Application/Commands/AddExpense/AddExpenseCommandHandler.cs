using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Application.Commands.AddExpense
{
    public class AddExpenseCommandHandler : IRequestHandler<AddExpenseCommand, int>
    {
        private readonly IExpenseWriter _expenseWriter;
        private readonly IValidator<AddExpenseCommand> _validator;
        private readonly ICurrentUserProvider _currentUserProvider;


        public AddExpenseCommandHandler(IExpenseWriter expenseWriter, IValidator<AddExpenseCommand> validator, ICurrentUserProvider currentUserProvider)
        {
            _expenseWriter = expenseWriter;
            _validator = validator;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<int> Handle(AddExpenseCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
              throw new ValidationException(validationResult.Errors);


            var currentUser = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);
            var expense = Expense.Create(request.Amount, request.Category, request.Description, request.Date, currentUser);
            int id = await _expenseWriter.AddAsync(expense, cancellationToken);
            return id;
        }
    }
}
