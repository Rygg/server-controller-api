using ServerController.Exceptions;
using ServerController.Models.AssettoController;

namespace ServerController.Interfaces
{
    /// <summary>
    /// Interface for AssettoCorsaServices.
    /// </summary>
    public interface IAssettoCorsaService
    {
        /// <summary>
        /// Method retrieves all the available tracks on the server.
        /// </summary>
        /// <returns>An enumerable list of all the available tracks and track configurations on the server.</returns>
        IEnumerable<string> GetTracks();
        /// <summary>
        /// Method starts the server with optional track configuration parameters.
        /// </summary>
        /// <param name="trackConfiguration">Nullable track configuration parameters.</param>
        /// <exception cref="InternalErrorException">Something went wrong with starting the server</exception>
        void StartServer(TrackConfiguration? trackConfiguration);
        /// <summary>
        /// Method stops the server if it's running.
        /// </summary>
        /// <exception cref="InternalErrorException">Something went wrong with starting the server</exception>
        void StopServer();
        /// <summary>
        /// Method first stops the server if it's running and then starts the server with optional track configuration parameters.
        /// </summary>
        /// <param name="trackConfiguration">Nullable track configuration parameters.</param>
        /// <exception cref="InternalErrorException">Something went wrong while restarting the server</exception>
        void RestartServer(TrackConfiguration? trackConfiguration);

    }
}
