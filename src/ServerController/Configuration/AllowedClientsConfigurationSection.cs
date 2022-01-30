namespace ServerController.Configuration
{
    /// <summary>
    /// Class model for allowed APIs configuration section.
    /// </summary>
    public class AllowedClientsConfigurationSection
    {
        /// <summary>
        /// Default ConfigurationSection name where the configuration should be.
        /// </summary>
        public const string DefaultConfigurationSectionName = "AllowedClients";

        /// <summary>
        /// Name of the client for logging purposes.
        /// </summary>
        public string ClientName { get; set; } = string.Empty;

        /// <summary>
        /// Client secret.
        /// </summary>
        public string ClientSecret { get; set; } = string.Empty;
        /// <summary>
        /// Is secret enabled
        /// </summary>
        public bool ClientAllowed { get; set; } = false;
    }
}
