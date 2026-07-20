using ExpenseTracker.Application.Commands.AddExpense;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpenseTracker.Application.Commands.AddExpensesBatch
{
    public class AddExpensesBatchCommandHandler : IRequestHandler<AddExpensesBatchCommand, AddExpensesBatchResult>
    {
        private readonly IExpenseWriter _expenseWriter;
        private readonly IReceiptWriter _receiptWriter;
        private readonly IValidator<AddExpenseCommand> _validator;
        private readonly ICurrentUserProvider _currentUserProvider;

        public AddExpensesBatchCommandHandler(IExpenseWriter expenseWriter, IReceiptWriter receiptWriter, IValidator<AddExpenseCommand> validator, ICurrentUserProvider currentUserProvider)
        {
            _expenseWriter = expenseWriter;
            _receiptWriter = receiptWriter;
            _validator = validator;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<AddExpensesBatchResult> Handle(AddExpensesBatchCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);
            var addedIds = new List<int>();
            var errors = new List<BatchItemError>();
            int? receiptId = null;

            for (int i = 0; i < request.Items.Count; i++)
            {
                var item = request.Items[i];
                var validationResult = await _validator.ValidateAsync(item, cancellationToken);

                if (!validationResult.IsValid)
                {
                    errors.Add(new BatchItemError(i, validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
                    continue;
                }

                if (receiptId is null)
                {
                    var receipt = Receipt.Create(DateTime.UtcNow);
                    receiptId = await _receiptWriter.AddAsync(receipt, cancellationToken);
                }

                var expense = Expense.Create(item.Amount, item.Category, item.Description, item.Date, currentUser, item.Merchant, receiptId);
                var id = await _expenseWriter.AddAsync(expense, cancellationToken);
                addedIds.Add(id);
            }

            return new AddExpensesBatchResult(addedIds, errors);
        }
    }
}
