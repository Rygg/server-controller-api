using ServerController.Interfaces;
using ServerController.Models.AssettoController;

namespace ServerController.Services
{
    /// <summary>
    /// Class implementing the <see cref="IAssettoCorsaService"/> interface. 
    /// </summary>
    public class AssettoCorsaService : IAssettoCorsaService
    {
        /// <summary>
        /// Retrieves all the available tracks on the server.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetTracksAsync()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Start the server with optional track configuration parameters.
        /// </summary>
        /// <param name="trackConfiguration">Nullable track configuration parameters.</param>
        /// <returns>Task representing the operation.</returns>
        public async Task StartServer(TrackConfiguration? trackConfiguration)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Stops and starts the server with optional track configuration parameters.
        /// </summary>
        /// <param name="trackConfiguration">Nullable track configuration parameters.</param>
        /// <returns>Task representing the operation.</returns>
        public async Task RestartServer(TrackConfiguration? trackConfiguration)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Stops the server.
        /// </summary>
        /// <returns>Task representing the operation.</returns>
        public async Task StopServer()
        {
            throw new NotImplementedException();
        }
    }
}
