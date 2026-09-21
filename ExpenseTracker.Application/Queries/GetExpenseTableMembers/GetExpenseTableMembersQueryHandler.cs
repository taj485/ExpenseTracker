using ExpenseTracker.Application.DTO;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ExpenseTracker.Application.Queries.GetExpenseTableMembers
{
    public class GetExpenseTableMembersQueryHandler : IRequestHandler<GetExpenseTableMembersQuery, IReadOnlyList<ExpenseTableMemberDto>>
    {
        private readonly IExpenseTableReader _expenseTableReader;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IValidator<GetExpenseTableMembersQuery> _validator;

        public GetExpenseTableMembersQueryHandler(IExpenseTableReader expenseTableReader, ICurrentUserProvider currentUserProvider, IValidator<GetExpenseTableMembersQuery> validator)
        {
            _expenseTableReader = expenseTableReader;
            _currentUserProvider = currentUserProvider;
            _validator = validator;
        }

        public async Task<IReadOnlyList<ExpenseTableMemberDto>> Handle(GetExpenseTableMembersQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var currentUser = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);

            // Non-members get the same 404 as a table that doesn't exist, so table ids can't be probed.
            if (!await _expenseTableReader.IsMemberAsync(request.ExpenseTableId, currentUser.Id, cancellationToken))
                throw new NotFoundException($"Expense table with id {request.ExpenseTableId} was not found");

            var members = await _expenseTableReader.GetMembersAsync(request.ExpenseTableId, cancellationToken);

            return members
                .Select(member => new ExpenseTableMemberDto
                {
                    UserId = member.UserId,
                    Email = member.Email,
                    IsAdmin = member.IsAdmin,
                    IsCurrentUser = member.UserId == currentUser.Id
                })
                .OrderByDescending(member => member.IsAdmin)
                .ThenBy(member => member.Email ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}
