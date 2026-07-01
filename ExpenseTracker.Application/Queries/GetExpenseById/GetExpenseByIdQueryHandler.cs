using ExpenseTracker.Application.DTO;
using ExpenseTracker.Application.Queries.GetAllExpenses;
using ExpenseTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Application.Queries.GetExpenseById
{
    public class GetExpenseByIdQueryHandler : IRequestHandler<GetExpenseByIdQuery, ExpenseDto>
    {
        private readonly IExpenseReader _expenseReader;
        private readonly GetExpenseByIdValidator _validator;

        public GetExpenseByIdQueryHandler(IExpenseReader expenseReader)
        {
            _expenseReader = expenseReader;
            _validator = new GetExpenseByIdValidator();
        }

        public async Task<ExpenseDto> Handle(GetExpenseByIdQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var expense = await _expenseReader.GetByIdAsync(request.Id, cancellationToken);

            if (expense is null)
                throw new NotFoundException($"Expense with id {request.Id} was not found");

            return new ExpenseDto
            {
                Id = expense.Id,
                Amount = expense.Amount.Amount,
                Currency = expense.Amount.Currency,
                Category = expense.Category.ToString(),
                Description = expense.Description,
                Date = expense.Date
            };
        }
    }
}
