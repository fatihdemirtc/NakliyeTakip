using MediatR;
using Microsoft.AspNetCore.Mvc;
using NakliyeTakip.Shared.Extensions;

namespace NakliyeTakip.Location.API.Features.Locations.GetCurrentLocation
{
    public static class InsertCurrentLocationQueryEndpoint
    {
        public static RouteGroupBuilder GetCurrentLocationEndpoint(this RouteGroupBuilder group)
        {
            group.MapGet("/GetCurrentLocation",
                    async ([FromServices] IMediator mediator) =>
                        (await mediator.Send(new InsertCurrentLocationQuery())).ToGenericResult())
                .WithName("GetCurrentLocation")
                .MapToApiVersion(1, 0);

            return group;
        }
    }
}
