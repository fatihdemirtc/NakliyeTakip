using Microsoft.Extensions.Logging;

namespace NakliyeTakip.MAUI.Services
{
    /// <summary>
    /// Example implementation for handling background location events.
    /// </summary>
    public interface ILocationEventHandler
    {
        /// <summary>
        /// Called when location is successfully sent.
        /// </summary>
        Task OnLocationSentAsync(double latitude, double longitude, DateTime timestamp);

        /// <summary>
        /// Called when there's an error sending location.
        /// </summary>
        Task OnLocationErrorAsync(Exception exception);
    }

    /// <summary>
    /// Example implementation of location event handler.
    /// </summary>
    public class LocationEventHandler : ILocationEventHandler
    {
        private readonly ILogger<LocationEventHandler> _logger;

        public LocationEventHandler(ILogger<LocationEventHandler> logger)
        {
            _logger = logger;
        }

        public Task OnLocationSentAsync(double latitude, double longitude, DateTime timestamp)
        {
            _logger.LogInformation($"Location successfully sent at {timestamp}: ({latitude}, {longitude})");
            
            // TODO: Update UI, save to database, etc.
            return Task.CompletedTask;
        }

        public Task OnLocationErrorAsync(Exception exception)
        {
            _logger.LogError(exception, "Failed to send location");
            
            // TODO: Show error notification, etc.
            return Task.CompletedTask;
        }
    }
}
