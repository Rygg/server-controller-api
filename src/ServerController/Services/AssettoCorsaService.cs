using ServerController.Interfaces;

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
    }
}
