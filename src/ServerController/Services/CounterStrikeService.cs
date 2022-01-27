using System.Diagnostics;
using Microsoft.Extensions.Options;
using ServerController.Configuration;
using ServerController.Exceptions;
using ServerController.Interfaces;
using ServerController.Utility;

namespace ServerController.Services
{    
    /// <summary>
    /// Class implementing the <see cref="ICounterStrikeService"/> interface. 
    /// </summary>
    public class CounterStrikeService : ICounterStrikeService
    {
        /// <summary>
        /// Injected logger.
        /// </summary>
        private readonly ILogger<CounterStrikeService> _logger;
        /// <summary>
        /// Server root directory from injected configuration.
        /// </summary>
        private readonly string _serverRootDirectory;
        /// <summary>
        /// Server executable directory parsed from injected configuration.
        /// </summary>
        private readonly string _serverExecutablePath;
        /// <summary>
        /// Server launch arguments from injected configuration.
        /// </summary>
        private readonly string _serverLaunchArguments;
        /// <summary>
        /// Server process name from injected configuration.
        /// </summary>
        private readonly string _serverProcessName;
        /// <summary>
        /// Private variable tracking the currently running server process.
        /// </summary>
        private Process? _serverProcess;

        /// <summary>
        /// Constructor to support dependency injections.
        /// </summary>
        /// <param name="logger">Injected logger</param>
        /// <param name="configuration">Injected configuration</param>
        public CounterStrikeService(ILogger<CounterStrikeService> logger, IOptions<CounterStrikeConfigurationSection> configuration)
        {
            _logger = logger;
            _serverRootDirectory = configuration.Value.ServerRootDirectory;
            _serverExecutablePath = Path.Combine(_serverRootDirectory, configuration.Value.ProcessExecutable);
            _serverLaunchArguments = configuration.Value.LaunchArguments;
            _serverProcessName = configuration.Value.ProcessName;

            ValidateConfiguration(); // Validate configuration.
        }

        /// <summary>
        /// Method throws an exception if a configuration value is invalid.
        /// </summary>
        /// <exception cref="ConfigurationException">Exception is thrown if configuration value is invalid</exception>
        private void ValidateConfiguration()
        {
            if (!Directory.Exists(_serverRootDirectory))
            {
                _logger.LogError("Root directory '{root}' not found", _serverRootDirectory);
                throw new ConfigurationException("Server root directory not located.");
            }
            if (!File.Exists(_serverExecutablePath))
            {
                _logger.LogError("Server executable file '{exe}' not found", _serverExecutablePath);
                throw new ConfigurationException("Server executable file not located.");
            }
        }

        /// <summary>
        /// Method starts the server.
        /// </summary>
        /// <exception cref="InternalErrorException">Something went wrong with starting the server</exception>
        public void StartServer()
        {
            try
            {
                if (ServerProcessUtilities.RefreshProcessAndCheckServerRunning(ref _serverProcess, _serverProcessName, _logger))
                {
                    _logger.LogInformation("Server is already running. Can't start.");
                    throw new InvalidOperationException("Server already running!");
                }

                ServerProcessUtilities.StartServerProcess(ref _serverProcess, _logger, _serverExecutablePath, _serverRootDirectory, _serverLaunchArguments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while starting the server process");
                throw new InternalErrorException("Something went wrong while starting the server process", ex);
            }
        }

        /// <summary>
        /// Method stops the server if it's running.
        /// </summary>
        /// <exception cref="InternalErrorException">Something went wrong with stopping the server</exception>
        public async Task StopServerAsync()
        {
            try
            {
                if (!ServerProcessUtilities.RefreshProcessAndCheckServerRunning(ref _serverProcess, _serverProcessName, _logger))
                {
                    _logger.LogInformation("Server is already stopped.");
                    return;
                }

                await ServerProcessUtilities.StopServerProcess(_serverProcess, _logger); // Server process is not null if this has been reached.
                
                _serverProcess?.Dispose(); // Dispose the process.
                _serverProcess = null; // Set to null.
                _logger.LogInformation("Server stopped.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while stopping the server process");
                throw new InternalErrorException("Something went wrong while stopping the server process", ex);
            }
        }
    }
}
