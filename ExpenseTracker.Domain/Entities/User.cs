using ExpenseTracker.Domain.Exceptions;

namespace ExpenseTracker.Domain.Entities
{
    public class User
    {
        public int Id { get; internal set; }
        public string Subject { get; private set; }
        public string? Email { get; private set; }
        public int? AiReceiptConsentVersion { get; private set; }
        public DateTimeOffset? AiReceiptConsentGivenAt { get; private set; }

        private User() { }

        public static User Create(string subject, string? email = null)
        {
            if (string.IsNullOrWhiteSpace(subject))
                throw new DomainException("Subject is required");

            return new User { Subject = subject, Email = Normalize(email) };
        }

        public void UpdateEmail(string? email)
        {
            Email = Normalize(email);
        }

        /// <summary>
        /// Records consent to sending receipt photos to the AI service. Giving the same version
        /// again keeps the original date.
        /// </summary>
        public void GiveAiReceiptConsent(int version, DateTimeOffset at)
        {
            if (AiReceiptConsentVersion == version)
                return;

            AiReceiptConsentVersion = version;
            AiReceiptConsentGivenAt = at;
        }

        public void WithdrawAiReceiptConsent()
        {
            AiReceiptConsentVersion = null;
            AiReceiptConsentGivenAt = null;
        }

        private static string? Normalize(string? email) => email?.Trim().ToLowerInvariant();
    }
}