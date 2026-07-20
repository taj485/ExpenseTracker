using ExpenseTracker.Application.DTO;
using ExpenseTracker.Domain.Interfaces;
using MediatR;

namespace ExpenseTracker.Application.Commands.ExtractReceiptExpenses
{
    public class ExtractReceiptExpensesCommandHandler : IRequestHandler<ExtractReceiptExpensesCommand, List<ExtractedExpenseDto>>
    {
        private readonly IReceiptExtractionService _receiptExtractionService;

        public ExtractReceiptExpensesCommandHandler(IReceiptExtractionService receiptExtractionService)
        {
            _receiptExtractionService = receiptExtractionService;
        }

        public async Task<List<ExtractedExpenseDto>> Handle(ExtractReceiptExpensesCommand request, CancellationToken cancellationToken)
        {
            var items = await _receiptExtractionService.ExtractAsync(request.ImageBytes, request.ContentType, cancellationToken);

            return items.Select(i => new ExtractedExpenseDto
            {
                Amount = i.Amount,
                Category = i.Category.ToString(),
                Description = i.Description,
                Date = i.Date,
                Quantity = i.Quantity,
                Merchant = i.Merchant,
            }).ToList();
        }
    }
}
