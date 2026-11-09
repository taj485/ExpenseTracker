using MediatR;

namespace ExpenseTracker.Application.Commands.UpdateReceiptDiscount
{
    public record UpdateReceiptDiscountCommand(decimal ReceiptDiscount, int ExpenseTableId = 0, int ReceiptId = 0) : IRequest<Unit>;
}
