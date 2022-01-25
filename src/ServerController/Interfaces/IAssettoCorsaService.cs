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
    }
}
