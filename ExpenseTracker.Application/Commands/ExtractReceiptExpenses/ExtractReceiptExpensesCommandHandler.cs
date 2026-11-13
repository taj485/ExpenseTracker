using ExpenseTracker.Application.DTO;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Domain.ValueObjects;
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
            var receipt = MoveReceiptDiscountOntoLoneItem(await _receiptExtractionService.ExtractAsync(request.ImageBytes, request.ContentType, cancellationToken));
            var items = receipt.Items;

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
                Unit = i.Unit.ToString(),
                Discount = i.Discount,
                ReceiptDiscount = receipt.ReceiptDiscount,
                // Prefer the reference table's spelling so the review drawer shows "Tesco",
                // not whatever the receipt happened to print.
                Merchant = known?.Name ?? i.Merchant,
                MerchantWebsite = known?.Website,
            }).ToList();
        }

        // A one-item receipt keeps all its discounts on the item, never more than the item's line total.
        private static ExtractedReceipt MoveReceiptDiscountOntoLoneItem(ExtractedReceipt receipt)
        {
            if (receipt.Items.Count != 1 || receipt.ReceiptDiscount == 0)
                return receipt;

            var item = receipt.Items[0];
            var discount = Math.Min(item.Discount + receipt.ReceiptDiscount, item.UnitPrice * item.Quantity);
            return new ExtractedReceipt(new[] { item with { Discount = discount } }, 0);
        }
    }
}
