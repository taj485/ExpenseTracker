using FluentValidation;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Application.Commands.AddExpense
{
    public class AddExpenseValidator : AbstractValidator<AddExpenseCommand>
    {
        public AddExpenseValidator()
        {
            RuleFor(x => x.ExpenseTableId)
                .GreaterThan(0).WithMessage("ExpenseTableId is required.");
            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Unit price must be greater than zero.");
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required");
            RuleFor(x => x.Category)
                .IsInEnum().WithMessage("Invalid expense category");
            RuleFor(x => x.Date.Date)
                .LessThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Date cannot be in the future.");
            RuleFor(x => x.Merchant)
                .MaximumLength(200).WithMessage("Merchant name is too long.");
            RuleFor(x => x.Discount)
                .GreaterThanOrEqualTo(0).WithMessage("Discount can't be negative.");
            RuleFor(x => x.Discount)
                .Must((command, discount) => discount <= command.UnitPrice * command.Quantity)
                .When(x => x.Discount >= 0)
                .WithMessage(x => $"Discount can't be more than the item total ({FormatPounds(x.UnitPrice * x.Quantity)}).");
        }

        /// <summary>Formats an amount the way validation messages show it, e.g. "£3.00".</summary>
        public static string FormatPounds(decimal amount)
        {
            return "£" + amount.ToString("0.00", CultureInfo.InvariantCulture);
        }
    }
}
