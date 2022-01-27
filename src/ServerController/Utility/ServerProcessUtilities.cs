﻿using System.Diagnostics;

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
                    logger.LogInformation("Assigning server process with PID {pId} to memory.", processes[0]);
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
    }
}