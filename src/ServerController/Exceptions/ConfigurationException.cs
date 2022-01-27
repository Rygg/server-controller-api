namespace ServerController.Exceptions
{
    /// <summary>
    /// Exception thrown when a server configuration is not acceptable.
    /// </summary>
    public class ConfigurationException : Exception
    {
        /// <summary>
        /// Constructor for the exception.
        /// </summary>
        /// <param name="msg"></param>
        public ConfigurationException(string msg) : base(msg) {}
    }
}
