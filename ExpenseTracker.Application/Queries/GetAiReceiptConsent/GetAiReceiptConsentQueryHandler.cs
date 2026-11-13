using ExpenseTracker.Application.DTO;
using ExpenseTracker.Application.Services;
using MediatR;

namespace ExpenseTracker.Application.Queries.GetAiReceiptConsent
{
    public class GetAiReceiptConsentQueryHandler : IRequestHandler<GetAiReceiptConsentQuery, AiReceiptConsentDto>
    {
        private readonly ICurrentUserProvider _currentUserProvider;

        public GetAiReceiptConsentQueryHandler(ICurrentUserProvider currentUserProvider)
        {
            _currentUserProvider = currentUserProvider;
        }

        public async Task<AiReceiptConsentDto> Handle(GetAiReceiptConsentQuery request, CancellationToken cancellationToken)
        {
            var user = await _currentUserProvider.GetOrProvisionAsync(cancellationToken);

            // A consent to an older text counts as not given.
            if (user.AiReceiptConsentVersion != AiReceiptConsent.CurrentVersion)
                return new AiReceiptConsentDto(false, null, null);

            return new AiReceiptConsentDto(true, user.AiReceiptConsentVersion, user.AiReceiptConsentGivenAt?.UtcDateTime);
        }
    }
}
