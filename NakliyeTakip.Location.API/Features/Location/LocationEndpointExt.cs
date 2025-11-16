using Asp.Versioning.Builder;
using NakliyeTakip.Location.API.Features.Locations.GetCurrentLocation;

namespace NakliyeTakip.Location.API.Features.Locations
{
    public static class LocationEndpointExt
    {
        public static void AddLocationGroupEndpointExt(this WebApplication app, ApiVersionSet apiVersionSet)
        {
            app.MapGroup("api/v{version:apiVersion}/locations").WithTags("locations").WithApiVersionSet(apiVersionSet)
                .GetCurrentLocationEndpoint()
                .RequireAuthorization();
        }
    }
}
