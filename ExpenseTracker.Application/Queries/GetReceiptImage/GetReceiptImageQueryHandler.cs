using ExpenseTracker.Application.DTO;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Application.Queries.GetReceiptImage
{
    public class GetReceiptImageQueryHandler : IRequestHandler<GetReceiptImageQuery, ReceiptImageDto>
    {

        private readonly IExpenseReader _expenseReader;
        private readonly IExpenseTableReader _expenseTableReader;
        private readonly IReceiptReader _receiptReader;
        private readonly IReceiptImageStore _receiptImageStore;
        private readonly ICurrentUserProvider _currentUserProvider;

        public GetReceiptImageQueryHandler(
            IExpenseReader expenseReader,
            IExpenseTableReader expenseTableReader,
            IReceiptReader receiptReader,
            IReceiptImageStore receiptImageStore,
            ICurrentUserProvider currentUserProvider)
        {
            _expenseReader = expenseReader;
            _expenseTableReader = expenseTableReader;
            _receiptReader = receiptReader;
            _receiptImageStore = receiptImageStore;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<ReceiptImageDto> Handle(GetReceiptImageQuery request, CancellationToken cancellationToken)
        {
            var currentUser = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);
            if (!await _expenseTableReader.IsMemberAsync(request.ExpenseTableId, currentUser.Id, cancellationToken))
                throw new NotFoundException($"Expense table with id {request.ExpenseTableId} was not found");

            var expenses = await _expenseReader.GetByReceiptIdAsync(request.ReceiptId, request.ExpenseTableId, cancellationToken);
            if(!expenses.Any())
                throw new NotFoundException($"Receipt with id {request.ReceiptId} was not found");

            var receipt = await _receiptReader.GetByIdAsync(request.ReceiptId, cancellationToken);
            if (receipt is null || string.IsNullOrWhiteSpace(receipt.ImageReference))
                throw new NotFoundException($"No image is available for receipt {request.ReceiptId}");

            var image = await _receiptImageStore.DownloadAsync(receipt.ImageReference, cancellationToken);
            if (image is null)
                throw new NotFoundException($"No image is available for receipt {request.ReceiptId}");

            return new ReceiptImageDto
            {
                Content = image.Content,
                ContentType = image.ContentType,
                FileName = $"receipt-{request.ReceiptId}{GetExtension(image.ContentType)}"
            };

        }

        private static string GetExtension(string contentType) => contentType switch
        {
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => ".jpg"
        };
    }
}
