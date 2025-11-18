namespace NakliyeTakip.MAUI.Services
{
    /// <summary>
    /// Background Location Service interface for sending location data in the background.
    /// </summary>
    public interface IBackgroundLocationService
    {
        /// <summary>
        /// Starts the background location tracking service.
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// Stops the background location tracking service.
        /// </summary>
        Task StopAsync();

        /// <summary>
        /// Gets the status of the background location service.
        /// </summary>
        /// <returns>True if the service is running, otherwise false.</returns>
        bool IsRunning { get; }
    }
}
