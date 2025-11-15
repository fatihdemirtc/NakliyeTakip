using Microsoft.Extensions.Logging;

namespace NakliyeTakip.MAUI.Services
{
    /// <summary>
    /// Location Service implementation for getting location data using MAUI Geolocation.
    /// </summary>
    public class LocationService : ILocationService
    {
        private readonly ILogger<LocationService> _logger;

        public LocationService(ILogger<LocationService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets the current location using MAUI's Geolocation API.
        /// </summary>
        public async Task<(double Latitude, double Longitude)> GetCurrentLocationAsync()
        {
            try
            {
                var hasPermission = await HasLocationPermissionAsync();
                if (!hasPermission)
                {
                    hasPermission = await RequestLocationPermissionAsync();
                }

                if (!hasPermission)
                {
                    _logger.LogWarning("Location permission not granted");
                    return (0, 0);
                }

                var location = await Geolocation.Default.GetLocationAsync(
                    new GeolocationRequest(
                        GeolocationAccuracy.Best,
                        TimeSpan.FromSeconds(10)));

                if (location is null)
                {
                    _logger.LogWarning("Location is null");
                    return (0, 0);
                }
                await AppShell.DisplayToastAsync("All cleaned up!");
                return (location.Latitude, location.Longitude);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                _logger.LogError(fnsEx, "Geolocation is not supported on this device");
                return (0, 0);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                _logger.LogError(fneEx, "Geolocation is not enabled");
                return (0, 0);
            }
            catch (PermissionException pEx)
            {
                _logger.LogError(pEx, "Location permission denied");
                return (0, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting location");
                return (0, 0);
            }
        }

        /// <summary>
        /// Checks if location permissions are granted.
        /// </summary>
        public async Task<bool> HasLocationPermissionAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            return status == PermissionStatus.Granted;
        }

        /// <summary>
        /// Requests location permissions from the user.
        /// </summary>
        public async Task<bool> RequestLocationPermissionAsync()
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            return status == PermissionStatus.Granted;
        }
    }
}
