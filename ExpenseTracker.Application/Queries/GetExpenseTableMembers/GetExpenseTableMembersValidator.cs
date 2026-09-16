using FluentValidation;

namespace ExpenseTracker.Application.Queries.GetExpenseTableMembers
{
    public class GetExpenseTableMembersValidator : AbstractValidator<GetExpenseTableMembersQuery>
    {
        public GetExpenseTableMembersValidator()
        {
            RuleFor(x => x.ExpenseTableId)
                .GreaterThan(0).WithMessage("ExpenseTableId is required.");
        }
    }
}
