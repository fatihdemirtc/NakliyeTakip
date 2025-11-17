using MediatR;
using NakliyeTakip.Location.API.Dto;
using NakliyeTakip.Shared;

namespace NakliyeTakip.Location.API.Features.Locations.GetCurrentLocation
{
    public class InsertCurrentLocationQueryHandler : IRequestHandler<InsertCurrentLocationQuery, ServiceResult<LocationDto>>
    {
        public async Task<ServiceResult<LocationDto>> Handle(InsertCurrentLocationQuery request, CancellationToken cancellationToken)
        {
            



            return ServiceResult<LocationDto>.SuccessAsOk(null);
        }
    }
}
