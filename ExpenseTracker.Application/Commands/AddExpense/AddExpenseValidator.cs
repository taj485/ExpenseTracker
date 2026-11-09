using ExpenseTracker.Domain.Entities;
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
            RuleFor(x => x.ExpenseTableId)
                .GreaterThan(0).WithMessage("ExpenseTableId is required.");
            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Unit price must be greater than zero.");
            RuleFor(x => x.Unit)
                .IsInEnum().WithMessage("Invalid quantity unit");
            RuleFor(x => x)
                .Must(x => Expense.QuantityError(x.Quantity, x.Unit) is null)
                .WithName(nameof(AddExpenseCommand.Quantity))
                .WithMessage(x => Expense.QuantityError(x.Quantity, x.Unit) + ".");
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
