using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ExpenseTracker.Application.Commands.UpdateReceiptDiscount
{
    public class UpdateReceiptDiscountCommandHandler : IRequestHandler<UpdateReceiptDiscountCommand, Unit>
    {
        private readonly IExpenseReader _expenseReader;
        private readonly IExpenseTableReader _expenseTableReader;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IValidator<UpdateReceiptDiscountCommand> _validator;

        public UpdateReceiptDiscountCommandHandler(IExpenseReader expenseReader, IExpenseTableReader expenseTableReader, ICurrentUserProvider currentUserProvider, IValidator<UpdateReceiptDiscountCommand> validator)
        {
            _expenseReader = expenseReader;
            _expenseTableReader = expenseTableReader;
            _currentUserProvider = currentUserProvider;
            _validator = validator;
        }

        public async Task<Unit> Handle(UpdateReceiptDiscountCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var currentUser = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);
            if (!await _expenseTableReader.IsMemberAsync(request.ExpenseTableId, currentUser.Id, cancellationToken))
                throw new NotFoundException($"Receipt with id {request.ReceiptId} was not found");

            var lines = (await _expenseReader.GetByReceiptIdAsync(request.ReceiptId, request.ExpenseTableId, cancellationToken)).ToList();
            var receipt = lines.FirstOrDefault()?.Receipt;

            if (receipt is null)
                throw new NotFoundException($"Receipt with id {request.ReceiptId} was not found");

            var totalAfterItemDiscounts = lines.Sum(e => e.UnitPrice.Amount * e.Quantity - e.Discount);
            receipt.UpdateDiscount(request.ReceiptDiscount, totalAfterItemDiscounts);

            await _expenseReader.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
