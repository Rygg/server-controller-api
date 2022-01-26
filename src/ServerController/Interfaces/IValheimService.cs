namespace ServerController.Interfaces
{
    /// <summary>
    /// Interface for ValheimServices
    /// </summary>
    public interface IValheimService
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
