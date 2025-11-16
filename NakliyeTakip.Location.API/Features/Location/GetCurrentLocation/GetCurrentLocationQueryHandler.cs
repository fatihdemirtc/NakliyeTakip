using MediatR;
using NakliyeTakip.Location.API.Dto;
using NakliyeTakip.Shared;

namespace NakliyeTakip.Location.API.Features.Locations.GetCurrentLocation
{
    public class GetCurrentLocationQueryHandler : IRequestHandler<GetCurrentLocationQuery, ServiceResult<LocationDto>>
    {
        public async Task<ServiceResult<LocationDto>> Handle(GetCurrentLocationQuery request, CancellationToken cancellationToken)
        {
            



            return ServiceResult<LocationDto>.SuccessAsOk(null);
        }
    }
}
