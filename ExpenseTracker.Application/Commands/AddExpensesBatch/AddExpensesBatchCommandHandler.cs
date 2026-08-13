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
        private readonly IReceiptImageStore _receiptImageStore;
        private readonly IValidator<AddExpenseCommand> _validator;
        private readonly ICurrentUserProvider _currentUserProvider;

        public AddExpensesBatchCommandHandler(IExpenseWriter expenseWriter, IReceiptWriter receiptWriter, IExpenseTableReader expenseTableReader, IReceiptImageStore receiptImageStore, IValidator<AddExpenseCommand> validator, ICurrentUserProvider currentUserProvider)
        {
            _expenseWriter = expenseWriter;
            _receiptWriter = receiptWriter;
            _expenseTableReader = expenseTableReader;
            _receiptImageStore = receiptImageStore;
            _validator = validator;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<AddExpensesBatchResult> Handle(AddExpensesBatchCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);

            if (!await _expenseTableReader.IsMemberAsync(request.ExpenseTableId, currentUser.Id, cancellationToken))
                throw new NotFoundException($"Expense table with id {request.ExpenseTableId} was not found");

            var addedIds = new List<int>();
            var errors = new List<BatchItemError>();
            int? receiptId = null;
            var imageReference = await PromoteImageAsync(request.TempImageReference, cancellationToken);

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
                    var receipt = Receipt.Create(DateTime.UtcNow, imageReference);
                    receiptId = await _receiptWriter.AddAsync(receipt, cancellationToken);
                }

                var expense = Expense.Create(item.Amount, item.Category, item.Description, item.Date, item.ExpenseTableId, item.Merchant, receiptId);
                var id = await _expenseWriter.AddAsync(expense, cancellationToken);
                addedIds.Add(id);
            }

            return new AddExpensesBatchResult(addedIds, errors);
        }

        /// <summary>
        /// Copies the receipt image out of the temp container into permanent storage. A missing or
        /// already-swept image must not fail the save — the expenses matter more than the photo.
        /// </summary>
        private async Task<string?> PromoteImageAsync(string? tempReference, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(tempReference))
                return null;

            try
            {
                return await _receiptImageStore.PromoteTempAsync(tempReference, cancellationToken);
            }
            catch
            {
                return null;
            }
        }
    }
}
