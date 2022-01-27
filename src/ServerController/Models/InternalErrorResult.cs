namespace ServerController.Models
{
    /// <summary>
    /// Model for objects returned when server encounters an internal error.
    /// </summary>
    public class InternalErrorResult
    {
        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// An optional field containing further information about the error.
        /// </summary>
        public string? Reason { get; set; }
    }
}
