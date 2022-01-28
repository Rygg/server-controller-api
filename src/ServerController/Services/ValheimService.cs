using System.Diagnostics;
using Microsoft.Extensions.Options;
using ServerController.Configuration;
using ServerController.Exceptions;
using ServerController.Interfaces;
using ServerController.Utility;

namespace ServerController.Services
{
    /// <summary>
    /// Class implementing the <see cref="IValheimService"/> interface. 
    /// </summary>
    public class ValheimService : IValheimService
    {
        /// <summary>
        /// The injected logger.
        /// </summary>
        private readonly ILogger<ValheimService> _logger;
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
        /// Should worlds be backed up on service close. Injected from configuration.
        /// </summary>
        private readonly bool _backupWorldsOnClose;
        /// <summary>
        /// Directory containing the server worlds from injected configuration.
        /// </summary>
        private readonly string _serverWorldDirectory;
        /// <summary>
        /// Prevent concurrent configuration world file backups.
        /// </summary>
        private readonly SemaphoreSlim _worldBackupLock;
        /// <summary>
        /// Timeout for world backup lock.
        /// </summary>
        private const int ConfigFileLockTimeoutMs = 1000; // 1 second.

        /// <summary>
        /// Private variable tracking the currently running server process.
        /// </summary>
        private Process? _serverProcess;

        /// <summary>
        /// Constructor for supporting dependency injections.
        /// </summary>
        /// <param name="logger">Injected logger</param>
        /// <param name="configuration">Injected configuration</param>
        public ValheimService(ILogger<ValheimService> logger, IOptions<ValheimConfigurationSection> configuration)
        {
            _logger = logger;
            _serverRootDirectory = configuration.Value.ServerRootDirectory;
            _serverExecutablePath = Path.Combine(_serverRootDirectory, configuration.Value.ProcessExecutable);
            _serverLaunchArguments = configuration.Value.LaunchArguments;
            _serverProcessName = configuration.Value.ProcessName;
            _serverWorldDirectory = configuration.Value.ServerWorldDirectory;
            _backupWorldsOnClose = configuration.Value.BackupWorldsOnClose;

            ValidateConfiguration(); // Validate configuration.

            _worldBackupLock = new SemaphoreSlim(1);
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

                if (_backupWorldsOnClose)
                {
                    await BackupServerWorldFiles(); // Back up the worlds before stopping the server.
                }

                await ServerProcessUtilities.StopServerProcess(_serverProcess, _logger);

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


        /// <summary>
        /// Method makes a couple of backups of the worlds, in case the server is randomly writing the world while the kill command is initiated or some copying is being performed.
        /// <remarks>This is required to prevent loss of world progression by killing the process in the middle of world saving.</remarks>
        /// </summary>
        private async Task BackupServerWorldFiles()
        {
            const int delayBetweenCopiesMs = 5000;

            const string backupDirectoryName = "worlds_backup1";
            const string backupDirectoryName2 = "worlds_backup2";

            if (!Directory.Exists(_serverWorldDirectory))
            {
                _logger.LogError("Server world directory '{worlds}' not found", _serverWorldDirectory);
                throw new ConfigurationException("Server world directory not located.");
            }

            var semaphoreEntered = false;
            try
            {
                semaphoreEntered = await _worldBackupLock.WaitAsync(ConfigFileLockTimeoutMs);
                if (!semaphoreEntered)
                {
                    throw new FileLoadException("World files were already being backed up");
                }

                var di = new DirectoryInfo(_serverWorldDirectory);
                if (di.Parent == null)
                {
                    _logger.LogError("Worlds directory did not contain a parent directory. Cannot perform server backup.");
                    throw new ArgumentException("Invalid folder structure.");
                }
                var backupDirectory1 = Path.Combine(di.Parent.FullName, backupDirectoryName);
                var backupDirectory2 = Path.Combine(di.Parent.FullName, backupDirectoryName2);

                if (!Directory.Exists(backupDirectory1))
                {
                    Directory.CreateDirectory(backupDirectory1);
                }
                if (!Directory.Exists(backupDirectory2))
                {
                    Directory.CreateDirectory(backupDirectory2);
                }

                var dirFilePaths = Directory.GetFiles(_serverWorldDirectory); // Get all the world files.
                _logger.LogDebug("Performing the first cycle of world backups.");
                foreach (var file in dirFilePaths)
                {
                    var targetPath = Path.Combine(backupDirectory1, Path.GetFileName(file));
                    _logger.LogTrace("Starting to copy {oldFile} to {newFile}", file, targetPath);
                    File.Copy(file, targetPath, true);
                }
                _logger.LogDebug("First backup cycle done.");
                _logger.LogDebug("Waiting {delay}ms to prevent world copying issues.", delayBetweenCopiesMs);
                await Task.Delay(delayBetweenCopiesMs);

                _logger.LogDebug("Performing the second cycle of world backups.");
                foreach (var file in dirFilePaths)
                {
                    var targetPath = Path.Combine(backupDirectory2, Path.GetFileName(file));
                    _logger.LogTrace("Starting to copy {oldFile} to {newFile}", file, targetPath);
                    File.Copy(file, targetPath, true);
                }
                _logger.LogDebug("Second backup cycle done.");
                _logger.LogInformation("Worlds backed up.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while backing up the world files");
                throw; // Rethrow to caller.
            }
            finally
            {
                if (semaphoreEntered)
                {
                    _worldBackupLock.Release(); // Release the semaphore if it was entered.
                }
            }
        }
    }
}
