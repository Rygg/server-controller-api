namespace ServerController.Interfaces
{
    /// <summary>
    /// Interface for CounterStrikeServices
    /// </summary>
    public interface ICounterStrikeService
    {   
        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <returns>Task representing the operation.</returns>
        Task StartServer();
        /// <summary>
        /// Stops the server.
        /// </summary>
        /// <returns>Task representing the operation.</returns>
        Task StopServer();
    }
}
