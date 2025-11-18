namespace NakliyeTakip.MAUI.Services
{
    /// <summary>
    /// Location Service interface for getting location data.
    /// </summary>
    public interface ILocationService
    {
        /// <summary>
        /// Gets the current location.
        /// </summary>
        /// <returns>A tuple containing latitude and longitude.</returns>
        Task<(double Latitude, double Longitude)> GetCurrentLocationAsync();

        /// <summary>
        /// Checks if location permissions are granted.
        /// </summary>
        /// <returns>True if permissions are granted, otherwise false.</returns>
        Task<bool> HasLocationPermissionAsync();

        /// <summary>
        /// Requests location permissions from the user.
        /// </summary>
        /// <returns>True if permissions were granted, otherwise false.</returns>
        Task<bool> RequestLocationPermissionAsync();
    }
}
