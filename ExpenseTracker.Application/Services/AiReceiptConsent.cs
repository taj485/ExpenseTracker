namespace ExpenseTracker.Application.Services
{
    /// <summary>
    /// The version of the AI receipt reading consent text the apps show. Raise it when the
    /// provider or what is sent changes, so everyone is asked again.
    /// </summary>
    public static class AiReceiptConsent
    {
        public const int CurrentVersion = 1;

        public const string OutOfDateMessage = "This consent text is out of date. Please update the app.";
    }
}
