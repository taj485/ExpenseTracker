using ExpenseTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ExpenseTracker.Application.Commands.DeleteExpense
{
    public class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand, Unit>
    {
        private readonly IExpenseWriter _expenseWriter;
        private readonly IValidator<DeleteExpenseCommand> _validator;

        public DeleteExpenseCommandHandler(IExpenseWriter expenseWriter, IValidator<DeleteExpenseCommand> validator)
        {
            _expenseWriter = expenseWriter;
            _validator = validator;
        }

        public async Task<Unit> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            await _expenseWriter.DeleteAsync(request.Id);

            return Unit.Value;
        }
    }
}
