using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Interfaces;
using MediatR;

namespace ExpenseTracker.Application.Commands.UploadReceiptImage
{
    public class UploadReceiptImageCommandHandler : IRequestHandler<UploadReceiptImageCommand, string>
    {
        private readonly IExpenseTableReader _expenseTableReader;
        private readonly IReceiptImageStore _receiptImageStore;
        private readonly ICurrentUserProvider _currentUserProvider;

        public UploadReceiptImageCommandHandler(IExpenseTableReader expenseTableReader, IReceiptImageStore receiptImageStore, ICurrentUserProvider currentUserProvider)
        {
            _expenseTableReader = expenseTableReader;
            _receiptImageStore = receiptImageStore;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<string> Handle(UploadReceiptImageCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);

            if (!await _expenseTableReader.IsMemberAsync(request.ExpenseTableId, currentUser.Id, cancellationToken))
                throw new NotFoundException($"Expense table with id {request.ExpenseTableId} was not found");

            return await _receiptImageStore.UploadAsync(request.ImageBytes, request.ContentType, cancellationToken);
        }
    }
}
