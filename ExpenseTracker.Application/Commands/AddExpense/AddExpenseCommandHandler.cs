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


        public AddExpenseCommandHandler(IExpenseWriter expenseWriter, IValidator<AddExpenseCommand> validator)
        {
            _expenseWriter = expenseWriter;
            _validator = validator;
        }

        public async Task<int> Handle(AddExpenseCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
              throw new ValidationException(validationResult.Errors);
            

            var expense = Expense.Create(request.Amount, request.Category, request.Description);
            int id = await _expenseWriter.AddAsync(expense, cancellationToken);
            return id;
        }
    }
}
