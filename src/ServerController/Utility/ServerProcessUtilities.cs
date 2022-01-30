using System.Diagnostics;

namespace ServerController.Utility
{
    /// <summary>
    /// Static class containing utility functions for controlling the Server process.
    /// </summary>
    internal static class ServerProcessUtilities
    {
        /// <summary>
        /// Method checks if the server process is stored in the memory. If not, the currently running processes are requested based on the serverProcessName-parameter.
        /// This can occur when application is restarted when a server is running.
        /// If there is a match, serverProcess is replaced with the first process in the result. If there is more than one result, a warning is logged.
        /// After refreshing the process variable. Method checks whether or not the process is running.
        /// </summary>
        /// <param name="serverProcess">Server process variable passed as a reference</param>
        /// <param name="serverProcessName">Process name used when requesting running processes.</param>
        /// <param name="logger">Logger to be used for logging.</param>
        /// <returns>A boolean value indicating if the server is currently running.</returns>
        internal static bool RefreshProcessAndCheckServerRunning(ref Process? serverProcess, string serverProcessName, ILogger logger)
        {
            // Check if serverProcess is in the memory.
            if (serverProcess == null)
            {
                logger.LogDebug("Stored server process was null, check running processes by name. ProcessName: {name}", serverProcessName);
                var processes = Process.GetProcessesByName(serverProcessName, Environment.MachineName);
                if (processes.Any())
                {
                    if (processes.Length > 1)
                    {
                        logger.LogWarning("Multiple running processes found. PIDs: {pIds} ", string.Join(", ", processes.Select(p => p.Id)));
                        logger.LogWarning("Selecting the first process.");
                    }
                    logger.LogInformation("Assigning server process with PID {pId} to memory.", processes[0].Id);
                    serverProcess = processes[0];
                }
                else
                {
                    logger.LogInformation("Server was not running.");
                    return false;
                }
            }

            serverProcess.Refresh(); // Refresh the process state.
            if (serverProcess.HasExited == false)
            {
                logger.LogInformation("Server is currently running with PID {pId}.", serverProcess.Id);
                return true;
            }
            return false; // Server is not running.
        }

        /// <summary>
        /// Method disposes the old serverProcess, creates a new process and signals for it to start.
        /// </summary>
        /// <param name="serverProcess">ServerProcess variable as a reference parameter.</param>
        /// <param name="logger">Logger</param>
        /// <param name="fileName">Process path to be started.</param>
        /// <param name="workingDirectory">Working directory for the process</param>
        /// <param name="arguments">Optional nullable launch arguments for the process</param>
        internal static void StartServerProcess(ref Process? serverProcess, ILogger logger, string fileName, string workingDirectory, string? arguments = null)
        {
            serverProcess?.Dispose(); // Dispose old process if it exists.
            serverProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    WorkingDirectory = workingDirectory,
                    Arguments = arguments,
                    UseShellExecute = false, // Don't shell execute.
                    CreateNoWindow = true, // No windows.
                }
            };
            logger.LogDebug("Starting new server process.");
            serverProcess.Start();
            logger.LogInformation("Server started with PID: {pId}", serverProcess.Id);
        }
        
        /// <summary>
        /// Method stops the server process given as the parameter.
        /// </summary>
        /// <param name="serverProcess">Server process to be stopped.</param>
        /// <param name="logger">Logger.</param>
        /// <returns></returns>
        internal static async Task StopServerProcess(Process? serverProcess, ILogger logger)
        {
            if (serverProcess == null)
            {
                throw new ArgumentNullException(nameof(serverProcess), "Server process was null.");
            }

            logger.LogDebug("Stopping server.");
            logger.LogDebug("Killing process with PID: {pId}", serverProcess.Id); // Process is not null.
            serverProcess.Kill();
            logger.LogTrace("Kill signal sent.");
            await serverProcess.WaitForExitAsync();
            logger.LogDebug("Process has stopped.");
        }
    }
}
