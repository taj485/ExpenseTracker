using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.DTO
{
    public static class ExpenseDtoMapper
    {
        public static ExpenseDto ToDto(this Expense expense, int currentUserId)
        {
            return new ExpenseDto
            {
                Id = expense.Id,
                UnitPrice = expense.UnitPrice.Amount,
                Quantity = expense.Quantity,
                Discount = expense.Discount,
                ReceiptDiscount = expense.Receipt?.Discount ?? 0,
                Currency = expense.UnitPrice.Currency,
                Category = expense.Category.ToString(),
                Description = expense.Description,
                Date = expense.Date,
                MerchantId = expense.MerchantId,
                Merchant = expense.Merchant?.Name,
                MerchantWebsite = expense.Merchant?.Website,
                ReceiptId = expense.ReceiptId,
                ExpenseTableId = expense.ExpenseTableId,
                CreatedByEmail = expense.CreatedByUser?.Email,
                CreatedByCurrentUser = expense.CreatedByUserId == currentUserId
            };
        }
    }
}
