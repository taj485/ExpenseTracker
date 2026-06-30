using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Application.Queries.GetExpenseById
{
    public class GetExpenseByIdValidator : AbstractValidator<GetExpenseByIdQuery>
    {
        public GetExpenseByIdValidator() 
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0.");
        }
    }
}
