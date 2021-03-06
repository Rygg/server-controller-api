using ServerController.Exceptions;

namespace ServerController.Interfaces
{
    /// <summary>
    /// Interface for CounterStrikeServices
    /// </summary>
    public interface ICounterStrikeService
    {
        /// <summary>
        /// Method starts the server.
        /// </summary>
        /// <exception cref="InternalErrorException">Something went wrong with starting the server</exception>
        void StartServer();
        /// <summary>
        /// Method stops the server if it's running.
        /// </summary>
        /// <exception cref="InternalErrorException">Something went wrong with stopping the server</exception>
        Task StopServerAsync();
    }
}
