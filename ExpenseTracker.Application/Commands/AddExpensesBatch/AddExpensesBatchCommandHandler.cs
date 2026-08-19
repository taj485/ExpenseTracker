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
        private readonly IExpenseTableReader _expenseTableReader;
        private readonly IValidator<AddExpenseCommand> _validator;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IMerchantResolver _merchantResolver;

        public AddExpensesBatchCommandHandler(IExpenseWriter expenseWriter, IReceiptWriter receiptWriter, IExpenseTableReader expenseTableReader, IValidator<AddExpenseCommand> validator, ICurrentUserProvider currentUserProvider, IMerchantResolver merchantResolver)
        {
            _expenseWriter = expenseWriter;
            _receiptWriter = receiptWriter;
            _expenseTableReader = expenseTableReader;
            _validator = validator;
            _currentUserProvider = currentUserProvider;
            _merchantResolver = merchantResolver;
        }

        public async Task<AddExpensesBatchResult> Handle(AddExpensesBatchCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);

            if (!await _expenseTableReader.IsMemberAsync(request.ExpenseTableId, currentUser.Id, cancellationToken))
                throw new NotFoundException($"Expense table with id {request.ExpenseTableId} was not found");

            // Every line item on a receipt shares one merchant, so resolve it once rather than per item.
            var merchantId = await _merchantResolver.ResolveOrCreateAsync(
                request.Items.Select(i => i.Merchant).FirstOrDefault(m => !string.IsNullOrWhiteSpace(m)),
                cancellationToken);

            var addedIds = new List<int>();
            var errors = new List<BatchItemError>();
            int? receiptId = null;

            for (int i = 0; i < request.Items.Count; i++)
            {
                var item = request.Items[i] with { ExpenseTableId = request.ExpenseTableId };
                var validationResult = await _validator.ValidateAsync(item, cancellationToken);

                if (!validationResult.IsValid)
                {
                    errors.Add(new BatchItemError(i, validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
                    continue;
                }

                if (receiptId is null)
                {
                    var receipt = Receipt.Create(DateTime.UtcNow, request.ReceiptImageReference);
                    receiptId = await _receiptWriter.AddAsync(receipt, cancellationToken);
                }

                var expense = Expense.Create(item.UnitPrice, item.Category, item.Description, item.Date, item.ExpenseTableId, merchantId, receiptId, item.Quantity);
                var id = await _expenseWriter.AddAsync(expense, cancellationToken);
                addedIds.Add(id);
            }

            return new AddExpensesBatchResult(addedIds, errors);
        }
    }
}
