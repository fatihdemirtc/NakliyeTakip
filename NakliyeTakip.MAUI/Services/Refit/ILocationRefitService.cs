using NakliyeTakip.MAUI.Dto;
using Refit;

namespace NakliyeTakip.MAUI.Services.Refit
{
    public interface ILocationRefitService
    {
        [Post("/v1/locations")]
        Task<ApiResponse<object>> InsertCurrentLocation(InsertCurrentLocationRequest request);

        [Get("/v1/locations/{id}")]
        Task<ApiResponse<LocationDto>> GetCurrentLocationByUser(Guid id);
    }
}
