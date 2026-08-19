using ExpenseTracker.Application.DTO;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Interfaces;
using MediatR;

namespace ExpenseTracker.Application.Commands.ExtractReceiptExpenses
{
    public class ExtractReceiptExpensesCommandHandler : IRequestHandler<ExtractReceiptExpensesCommand, List<ExtractedExpenseDto>>
    {
        private readonly IReceiptExtractionService _receiptExtractionService;
        private readonly IMerchantResolver _merchantResolver;

        public ExtractReceiptExpensesCommandHandler(IReceiptExtractionService receiptExtractionService, IMerchantResolver merchantResolver)
        {
            _receiptExtractionService = receiptExtractionService;
            _merchantResolver = merchantResolver;
        }

        public async Task<List<ExtractedExpenseDto>> Handle(ExtractReceiptExpensesCommand request, CancellationToken cancellationToken)
        {
            var items = await _receiptExtractionService.ExtractAsync(request.ImageBytes, request.ContentType, cancellationToken);

            // All items come from one receipt and share a merchant, so this is a single lookup.
            // Lookup only — the user can still cancel, and nothing should be written until they save.
            var rawMerchant = items.Select(i => i.Merchant).FirstOrDefault(m => !string.IsNullOrWhiteSpace(m));
            var known = await _merchantResolver.FindAsync(rawMerchant, cancellationToken);

            return items.Select(i => new ExtractedExpenseDto
            {
                UnitPrice = i.UnitPrice,
                Category = i.Category.ToString(),
                Description = i.Description,
                Date = i.Date,
                Quantity = i.Quantity,
                // Prefer the reference table's spelling so the review drawer shows "Tesco",
                // not whatever the receipt happened to print.
                Merchant = known?.Name ?? i.Merchant,
                MerchantWebsite = known?.Website,
            }).ToList();
        }
    }
}
