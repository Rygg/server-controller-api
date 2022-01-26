using ServerController.Models.AssettoController;

namespace ServerController.Interfaces
{
    /// <summary>
    /// Interface for AssettoCorsaServices.
    /// </summary>
    public interface IAssettoCorsaService
    {
        /// <summary>
        /// Retrieve all available tracks on the server.
        /// </summary>
        /// <returns>A list of all available tracks and track configurations on the server.</returns>
        Task<IEnumerable<string>> GetTracksAsync();
        /// <summary>
        /// Start the server with optional track configuration parameters.
        /// </summary>
        /// <param name="trackConfiguration">Nullable track configuration parameters.</param>
        /// <returns>Task representing the operation.</returns>
        Task StartServer(TrackConfiguration? trackConfiguration);
        /// <summary>
        /// Stops and starts the server with optional track configuration parameters.
        /// </summary>
        /// <param name="trackConfiguration">Nullable track configuration parameters.</param>
        /// <returns>Task representing the operation.</returns>
        Task RestartServer(TrackConfiguration? trackConfiguration);
        /// <summary>
        /// Stops the server.
        /// </summary>
        /// <returns>Task representing the operation.</returns>
        Task StopServer();
    }
}
