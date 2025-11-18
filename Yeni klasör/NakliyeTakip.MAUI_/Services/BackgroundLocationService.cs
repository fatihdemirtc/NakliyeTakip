using Microsoft.Extensions.Logging;

namespace NakliyeTakip.MAUI.Services
{
    /// <summary>
    /// Background Location Service implementation for sending location data every 10 seconds.
    /// </summary>
    public class BackgroundLocationService : IBackgroundLocationService
    {
        private readonly ILocationService _locationService;
        private readonly ILogger<BackgroundLocationService> _logger;
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _backgroundTask;

        public bool IsRunning => _cancellationTokenSource != null && !_cancellationTokenSource.Token.IsCancellationRequested;

        public BackgroundLocationService(ILocationService locationService, ILogger<BackgroundLocationService> logger)
        {
            _locationService = locationService;
            _logger = logger;
        }

        /// <summary>
        /// Starts the background location tracking service.
        /// The service sends location data every 10 seconds.
        /// </summary>
        public async Task StartAsync()
        {
            if (IsRunning)
            {
                _logger.LogWarning("Background location service is already running");
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _backgroundTask = RunBackgroundLocationTracking(_cancellationTokenSource.Token);

            _logger.LogInformation("Background location service started");
            await Task.CompletedTask;
        }

        /// <summary>
        /// Stops the background location tracking service.
        /// </summary>
        public async Task StopAsync()
        {
            if (!IsRunning)
            {
                _logger.LogWarning("Background location service is not running");
                return;
            }

            _cancellationTokenSource?.Cancel();
            
            if (_backgroundTask != null)
            {
                try
                {
                    await _backgroundTask;
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested
                }
            }

            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            _backgroundTask = null;

            _logger.LogInformation("Background location service stopped");
        }

        /// <summary>
        /// Runs the background location tracking loop.
        /// </summary>
        private async Task RunBackgroundLocationTracking(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        // Get current location
                        var (latitude, longitude) = await _locationService.GetCurrentLocationAsync();

                        // Send location to server
                        await SendLocationToServerAsync(latitude, longitude, cancellationToken);

                        _logger.LogInformation($"Location sent: Lat={latitude}, Lon={longitude}");

                        // Wait 10 seconds before next location update
                        await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        throw; // Re-throw to exit the loop
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in background location tracking");
                        // Continue on error
                        await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Background location tracking was cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in background location tracking");
            }
        }

        /// <summary>
        /// Sends the location data to the server.
        /// TODO: Implement actual server communication here.
        /// </summary>
        private async Task SendLocationToServerAsync(double latitude, double longitude, CancellationToken cancellationToken)
        {
            try
            {
                // TODO: Replace with actual server endpoint
                const string serverUrl = "https://your-server.com/api/location";

                using var httpClient = new HttpClient();
                
                var locationData = new
                {
                    latitude,
                    longitude,
                    timestamp = DateTime.UtcNow
                };

                var jsonContent = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(locationData),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await httpClient.PostAsync(serverUrl, jsonContent, cancellationToken);
                response.EnsureSuccessStatusCode();

                _logger.LogDebug($"Location sent successfully to server");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "Failed to send location to server");
                // Continue on network error, will retry in next iteration
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending location to server");
            }
        }
    }
}
