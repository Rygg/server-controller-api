namespace ServerController.Exceptions
{
    /// <summary>
    /// Exception thrown when something goes wrong within the Service processing.
    /// </summary>
    public class InternalErrorException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="innerException"></param>
        public InternalErrorException(string msg, Exception innerException) : base(msg, innerException) {}
    }
}
