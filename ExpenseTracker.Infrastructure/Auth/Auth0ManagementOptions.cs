namespace ExpenseTracker.Infrastructure.Auth
{
    /// <summary>
    /// A machine-to-machine Auth0 application with only the delete:users scope.
    /// ClientId and ClientSecret come from user secrets / Azure app settings, never appsettings.json.
    /// </summary>
    public class Auth0ManagementOptions
    {
        public string Domain { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }
}
