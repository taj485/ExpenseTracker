using MediatR;

namespace ExpenseTracker.Application.Commands.UploadReceiptImage
{
    public record UploadReceiptImageCommand(int ExpenseTableId, byte[] ImageBytes, string ContentType) : IRequest<string>;
}
