using ExpenseTracker.Application.DTO;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Interfaces;
using MediatR;

namespace ExpenseTracker.Application.Queries.GetExpenseTablesForUser
{
    public class GetExpenseTablesForUserQueryHandler : IRequestHandler<GetExpenseTablesForUserQuery, IReadOnlyList<ExpenseTableDto>>
    {
        private readonly IExpenseTableReader _expenseTableReader;
        private readonly ICurrentUserProvider _currentUserProvider;

        public GetExpenseTablesForUserQueryHandler(IExpenseTableReader expenseTableReader, ICurrentUserProvider currentUserProvider)
        {
            _expenseTableReader = expenseTableReader;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<IReadOnlyList<ExpenseTableDto>> Handle(GetExpenseTablesForUserQuery request, CancellationToken cancellationToken)
        {
            var currentUser = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);
            var tables = await _expenseTableReader.GetAllForUserAsync(currentUser.Id, cancellationToken);

            return tables
                .Select(table => new ExpenseTableDto
                {
                    Id = table.Id,
                    Name = table.Name,
                    DateCreated = table.DateCreated,
                    IsCurrentUserAdmin = table.Members.Any(m => m.UserId == currentUser.Id && m.IsAdmin),
                    MemberCount = table.Members.Count
                }).ToList();
        }
    }
}
