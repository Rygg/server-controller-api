namespace ServerController.Configuration
{
    /// <summary>
    /// Class model for CounterStrike server configuration sections.
    /// </summary>
    public class CounterStrikeConfigurationSection
    {
        /// <summary>
        /// Default ConfigurationSection name where the configuration is expected to be.
        /// </summary>
        public const string DefaultConfigurationSectionName = "CounterStrike";

        /// <summary>
        /// Server root directory location.
        /// </summary>
        public string ServerRootDirectory { get; set; } = string.Empty;
        /// <summary>
        /// Process name of the server process.
        /// </summary>
        public string ProcessName { get; set; } = "srcds";
        /// <summary>
        /// Process executable path.
        /// </summary>
        public string ProcessExecutable { get; set; } = "srcds.exe";

        /// <summary>
        /// Process launch arguments.
        /// </summary>
        public string LaunchArguments { get; set; } = "-game csgo";
    }
}
