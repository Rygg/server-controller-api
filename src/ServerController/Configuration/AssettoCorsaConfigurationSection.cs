namespace ServerController.Configuration
{
    /// <summary>
    /// Class model for AssettoCorsa configuration sections.
    /// </summary>
    public class AssettoCorsaConfigurationSection
    {
        /// <summary>
        /// Default ConfigurationSection name where the configuration should be.
        /// </summary>
        public const string DefaultConfigurationSectionName = "AssettoCorsa";

        /// <summary>
        /// Server root directory location.
        /// </summary>
        public string ServerRootDirectory { get; set; } = string.Empty;

        /// <summary>
        /// Path to the sub-directory containing all the tracks.
        /// </summary>
        public string TracksLocation { get; set; } = "content\\tracks\\";

        /// <summary>
        /// Server configuration file location.
        /// </summary>
        public string ServerConfigurationFile { get; set; } = "cfg\\server_cfg.ini";

        /// <summary>
        /// Process name of the server process.
        /// </summary>
        public string ProcessName { get; set; } = "acServer";

        /// <summary>
        /// Process executable path.
        /// </summary>
        public string ProcessExecutable { get; set; } = "acServer.exe";
    }
}
