using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Application.Commands.AddExpense
{
    public class AddExpenseValidator : AbstractValidator<AddExpenseCommand>
    {
        public AddExpenseValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required");
            RuleFor(x => x.Category)
                .IsInEnum().WithMessage("Invalid expense category");
            RuleFor(x => x.Date.Date)
                .LessThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Date cannot be in the future.");
            RuleFor(x => x.Merchant)
                .MaximumLength(200).WithMessage("Merchant name is too long.");
        }
    }
}
