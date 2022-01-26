namespace ServerController.Models.AssettoController
{
    /// <summary>
    /// TrackConfiguration model for requests starting or restarting the Assetto Corsa server.
    /// </summary>
    public class TrackConfiguration
    {
        /// <summary>
        /// Name of the track. Required if TrackConfiguration is present.
        /// </summary>
        public string Track { get; set; }
        /// <summary>
        /// Configuration of the track. Nullable.
        /// </summary>
        public string? Configuration { get; set; }
        /// <summary>
        /// Returns the track configuration as a readable string value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Track}, {Configuration}";
        }
    }
}
