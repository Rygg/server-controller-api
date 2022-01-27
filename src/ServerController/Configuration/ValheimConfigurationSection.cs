namespace ServerController.Configuration
{
    /// <summary>
    /// Class model for Valheim server configuration sections.
    /// </summary>
    public class ValheimConfigurationSection
    {
        /// <summary>
        /// Default ConfigurationSection name where the configuration is expected to be.
        /// </summary>
        public const string DefaultConfigurationSectionName = "Valheim";

        /// <summary>
        /// Server root directory location.
        /// </summary>
        public string ServerRootDirectory { get; set; } = string.Empty;
        /// <summary>
        /// Process name of the server process.
        /// </summary>
        public string ProcessName { get; set; } = "valheim_server";
        /// <summary>
        /// Process executable path.
        /// </summary>
        public string ProcessExecutable { get; set; } = "valheim_server.exe";

        /// <summary>
        /// Process launch arguments.
        /// </summary>
        public string LaunchArguments { get; set; } = "-nographics";

        /// <summary>
        /// Directory containing worlds present on the server.
        /// </summary>
        public string ServerWorldDirectory { get; set; } = string.Empty;
    }
}
