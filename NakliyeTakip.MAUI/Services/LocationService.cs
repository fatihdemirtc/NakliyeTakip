using NakliyeTakip.MAUI.Dto;
using NakliyeTakip.MAUI.Services.Refit;

namespace NakliyeTakip.MAUI.Services
{
    public class LocationService(ILocationRefitService locationRefitService)
    {
        public async Task<object> InsertCurrentLocation(InsertCurrentLocationRequest request)
        {
            var response = await locationRefitService.InsertCurrentLocation(request);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult<LocationDto>> GetCurrentLocationByUser(Guid courseId)
        {
            var response = await locationRefitService.GetCurrentLocationByUser(courseId);

            if (!response.IsSuccessStatusCode)
                return ServiceResult<LocationDto>.FailFromProblemDetails(response.Error);


            var location = response.Content!;
            var locationViewModel = new LocationDto(location.LastSeen, location.Latitude, location.Longitude);

            return ServiceResult<LocationDto>.Success(locationViewModel);
        }


    }
}
