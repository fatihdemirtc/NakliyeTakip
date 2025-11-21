using NakliyeTakip.MAUI.Dto;
using Refit;

namespace NakliyeTakip.MAUI.Services.Refit
{
    public interface ILocationRefitService
    {
        [Post("/api/v1/locations")]
        Task<ApiResponse<object>> InsertCurrentLocation(InsertCurrentLocationRequest request);
    }
}
