using MediatR;
using NakliyeTakip.Shared.Extensions;

namespace NakliyeTakip.Location.API.Features.Locations.GetCurrentLocation
{
    public static class GetCurrentLocationQueryEndpoint
    {
        public static RouteGroupBuilder GetCurrentLocationEndpoint(this RouteGroupBuilder group)
        {
            group.MapGet("/",
                    async (IMediator mediator) =>
                        (await mediator.Send(new GetCurrentLocationQuery())).ToGenericResult())
                .WithName("GetCurrentLocation")
                .MapToApiVersion(1, 0);

            return group;
        }
    }
}
