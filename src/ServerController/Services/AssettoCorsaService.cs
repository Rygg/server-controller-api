using System.Diagnostics;
using Microsoft.Extensions.Options;
using ServerController.Configuration;
using ServerController.Exceptions;
using ServerController.Interfaces;
using ServerController.Models.AssettoController;
using ServerController.Utility;

namespace ServerController.Services
{
    /// <summary>
    /// Class implementing the <see cref="IAssettoCorsaService"/> interface. 
    /// </summary>
    public class AssettoCorsaService : IAssettoCorsaService
    {
        /// <summary>
        /// Injected logger.
        /// </summary>
        private readonly ILogger<AssettoCorsaService> _logger;
        /// <summary>
        /// Server root directory from injected configuration.
        /// </summary>
        private readonly string _serverRootDirectory;
        /// <summary>
        /// Server tracks directory parsed from injected configuration.
        /// </summary>
        private readonly string _tracksDirectoryPath;
        /// <summary>
        /// Server executable directory parsed from injected configuration.
        /// </summary>
        private readonly string _serverExecutablePath;
        /// <summary>
        /// Server configuration file parsed from injected configuration.
        /// </summary>
        private readonly string _serverConfigurationFilePath;
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
        /// <param name="logger">Injected logger.</param>
        /// <param name="configuration">Injected configuration.</param>
        public AssettoCorsaService(ILogger<AssettoCorsaService> logger, IOptions<AssettoCorsaConfigurationSection> configuration)
        {
            _logger = logger;
            _serverRootDirectory = configuration.Value.ServerRootDirectory;
            _serverProcessName = configuration.Value.ProcessName;
            _tracksDirectoryPath = Path.Combine(_serverRootDirectory, configuration.Value.TracksLocation);
            _serverExecutablePath = Path.Combine(_serverRootDirectory, configuration.Value.ProcessExecutable);
            _serverConfigurationFilePath = Path.Combine(_serverRootDirectory, configuration.Value.ServerConfigurationFile);

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
                throw new ConfigurationException("Assetto Corsa root directory not located.");
            }
            if (!Directory.Exists(_tracksDirectoryPath))
            {
                _logger.LogError("Tracks directory '{tracks}' not found", _tracksDirectoryPath);
                throw new ConfigurationException("Assetto Corsa tracks directory not located.");
            }
            if (!File.Exists(_serverConfigurationFilePath))
            {
                _logger.LogError("Server configuration file '{config}' not found", _serverConfigurationFilePath);
                throw new ConfigurationException("Assetto Corsa server configuration file not located.");
            }
            if (!File.Exists(_serverExecutablePath))
            {
                _logger.LogError("Server executable file '{exe}' not found", _serverExecutablePath);
                throw new ConfigurationException("Assetto Corsa server executable file not located.");
            }
        }

        /// <summary>
        /// Method retrieves all the available tracks on the server.
        /// </summary>
        /// <returns>An enumerable list of all the available tracks and track configurations on the server.</returns>
        public IEnumerable<string> GetTracks()
        {
            return GetAvailableTracks();
        }

        /// <summary>
        /// Method starts the server with optional track configuration parameters.
        /// </summary>
        /// <param name="trackConfiguration">Nullable track configuration parameters.</param>
        /// <exception cref="InternalErrorException">Something went wrong with starting the server</exception>
        public void StartServer(TrackConfiguration? trackConfiguration)
        {
            try
            {
                if (ServerProcessUtilities.RefreshProcessAndCheckServerRunning(ref _serverProcess, _serverProcessName, _logger))
                {
                    _logger.LogInformation("Server is already running. Can't start.");
                    throw new InvalidOperationException("Server already running!");
                }

                if (trackConfiguration != null)
                {
                    _logger.LogDebug("Track configuration received.");
                    UpdateServerTrackConfiguration(trackConfiguration.Track, trackConfiguration.Configuration);
                }

                _serverProcess?.Dispose(); // Dispose old process if it exists for some reason. Never should though.
                _serverProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _serverExecutablePath,
                        WorkingDirectory = _serverRootDirectory,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };
                _logger.LogDebug("Starting new server process.");
                _serverProcess.Start();
                _logger.LogInformation("Server started with PID: {pId}", _serverProcess.Id);
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
        /// <exception cref="InternalErrorException">Something went wrong with starting the server</exception>
        public void StopServer()
        {
            try
            {
                if (!ServerProcessUtilities.RefreshProcessAndCheckServerRunning(ref _serverProcess, _serverProcessName, _logger))
                {
                    _logger.LogInformation("Server is already stopped.");
                    return;
                }

                // Otherwise server is running.
                _logger.LogDebug("Stopping server.");
                _logger.LogDebug("Killing process with PID: {pId}", _serverProcess!.Id); // Process is not null.
                _serverProcess.Kill();
                _logger.LogTrace("Kill signal sent.");
                _serverProcess.WaitForExit();
                _logger.LogDebug("Process has stopped.");

                _serverProcess.Dispose(); // Dispose the process.
                _serverProcess = null; // Set to null.
                _logger.LogInformation("Server stopped.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while stopping the server process");
                throw new InternalErrorException("Something went wrong while stopping the server process", ex);
            }
        }

        /// <summary>
        /// Method first stops the server if it's running and then starts the server with optional track configuration parameters.
        /// </summary>
        /// <param name="trackConfiguration">Nullable track configuration parameters.</param>
        /// <exception cref="InternalErrorException">Something went wrong while restarting the server</exception>
        public void RestartServer(TrackConfiguration? trackConfiguration)
        {
            StopServer();
            StartServer(trackConfiguration);
        }

        /// <summary>
        /// Method returns all available tracks on the server.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> GetAvailableTracks()
        {
            const string dataDirectoryName = "data";

            _logger.LogDebug("Retrieving available tracks from server locations.");

            var locatedTracks = new List<string>();

            var subDirs = Directory.GetDirectories(_tracksDirectoryPath); // Get all subdirectories from tracks.

            foreach (var subDir in subDirs)
            {
                var trackName = new DirectoryInfo(subDir).Name; // Track name is the same as the directory name.
                _logger.LogTrace("Track: {subDir}", subDir);
                // Retrieve all track configurations. Track configurations are stored in the track directories subdirectories, with the exception of data directory:
                var configs = Directory.GetDirectories(subDir).Where(c => new DirectoryInfo(c).Name != dataDirectoryName).ToArray();
                if (configs.Any())
                {
                    _logger.LogTrace("Track contained subdirectories: {subDirs}", string.Join(", ", configs));
                    // Track configurations found. Add track all track combinations to the return list.
                    var trackConfigs = configs.Select(c => $"{trackName} {new DirectoryInfo(c).Name}");
                    locatedTracks.AddRange(trackConfigs);
                }
                else
                {
                    _logger.LogTrace("Track contained no subdirectories");
                    locatedTracks.Add(trackName);
                }
            }

            _logger.LogInformation("Found {count} tracks on the server.", locatedTracks.Count);
            _logger.LogDebug("Tracks: {tracks}", string.Join(", ", locatedTracks));
            return locatedTracks;
        }

        /// <summary>
        /// Method updates the server configuration file with the given parameters.
        /// </summary>
        /// <param name="track">Track to be set</param>
        /// <param name="configuration">Track configuration to be set</param>
        /// <exception cref="ArgumentException">Track configuration parameters were invalid</exception>
        private void UpdateServerTrackConfiguration(string track, string? configuration)
        {
            const string trackTag = "TRACK=";
            const string configTag = "CONFIG_TRACK=";

            _logger.LogDebug("Updating server track configuration");

            try
            {
                if (!ValidateTrackConfiguration(track, configuration))
                {
                    throw new ArgumentException("Track configuration not valid.");
                }

                var configLines = File.ReadAllLines(_serverConfigurationFilePath); // Read all lines from the server configuration file..

                var trackUpdated = false;
                var trackConfigurationUpdated = false;
                // Loop through the lines.
                for (var i = 0; i < configLines.Length; i++)
                {
                    if (trackUpdated && trackConfigurationUpdated) // Both track and track configuration was updated.
                    {
                        _logger.LogTrace("Saving file changes.");
                        File.WriteAllLines(_serverConfigurationFilePath, configLines); // Save the configuration file.
                        _logger.LogDebug("Server configuration file saved.");
                        break; // break the loop.
                    }

                    if (!trackUpdated && configLines[i].StartsWith(trackTag)) // See if track was not yet updated and the current line starts with the predetermined tag.
                    {
                        configLines[i] = trackTag + track; // TRACK=track 
                        trackUpdated = true; // Set track as updated.
                        _logger.LogDebug("Updated configuration file line with new track. Line value: {newLine}", configLines[i]);
                        continue;
                    }

                    if (!trackConfigurationUpdated && configLines[i].StartsWith(configTag)) // See if the track configuration should be updated.
                    {
                        configLines[i] = configTag + configuration; // CONFIG_TAG=Configuration or CONFIG_TAG=
                        trackConfigurationUpdated = true;
                        _logger.LogDebug("Updated configuration file line with new track configuration. Line value: {newLine}", configLines[i]);
                        continue;
                    }
                }

                _logger.LogInformation("Server configuration file updated.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while updating track configuration");
                throw; // Rethrow to caller.
            }
        }

        /// <summary>
        /// Method validates track configuration exists.
        /// </summary>
        /// <param name="track"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private bool ValidateTrackConfiguration(string track, string? config)
        {
            var availableTracks = GetAvailableTracks(); // Get all available tracks.
            var trackConfigurationString = string.IsNullOrEmpty(config) ? string.Empty : $" {config}"; // Track configuration is either empty or " config".

            if (!availableTracks.Contains(track + trackConfigurationString))
            {
                _logger.LogError("Track or track configuration not available. Attempted configuration: '{conf}'", trackConfigurationString);
                return false; // Track not available.
            }
            return true;
        }
    }
}
