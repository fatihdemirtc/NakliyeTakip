using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NakliyeTakip.Location.API.Dto;
using NakliyeTakip.Location.API.Repositories;
using NakliyeTakip.Shared;
using NakliyeTakip.Shared.Extensions;
using System.Net;

namespace NakliyeTakip.Location.API.Features.Locations.GetCurrentLocation
{
    public record GetCurrentLocationByUserQuery(Guid Id) : IRequestByServiceResult<LocationDto>;

    public class GetCurrentLocationByUserQueryHandler(AppDbContext context, IMapper mapper)
       : IRequestHandler<GetCurrentLocationByUserQuery, ServiceResult<LocationDto>>
    {
        public async Task<ServiceResult<LocationDto>> Handle(GetCurrentLocationByUserQuery request,
            CancellationToken cancellationToken)
        {
            var lastLocation = await context.Locations
                .Where(x => x.UserId == request.Id)
                .OrderByDescending(x => x.Created)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastLocation is null)
            {
                return ServiceResult<LocationDto>.Error("location not found",
                    $"The last location with userId({request.Id}) was not found", HttpStatusCode.NotFound);
            }

            // Ensure LastSeen reflects the timestamp of the latest location.
            var locationAsDto = new LocationDto
            {
                LastSeen = lastLocation.Created,
                Latitude = lastLocation.Latitude,
                Longitude = lastLocation.Longitude
            };

            return ServiceResult<LocationDto>.SuccessAsOk(locationAsDto);
        }
    }

    public static class GetCurrentLocationByUserEndpoint
    {
        public static RouteGroupBuilder GetCurrentLocationByUserGroupItemEndpoint(this RouteGroupBuilder group)
        {
            group.MapGet("/{id:guid}",
                    async (IMediator mediator, Guid id) =>
                        (await mediator.Send(new GetCurrentLocationByUserQuery(id))).ToGenericResult())
                .WithName("GetCurrentLocation")
                .MapToApiVersion(1, 0);

            return group;
        }
    }

   
}
