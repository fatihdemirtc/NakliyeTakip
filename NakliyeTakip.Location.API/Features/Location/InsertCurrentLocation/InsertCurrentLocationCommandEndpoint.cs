using MediatR;
using Microsoft.AspNetCore.Mvc;
using NakliyeTakip.Shared.Extensions;
using NakliyeTakip.Shared.Filters;

namespace NakliyeTakip.Location.API.Features.Location.InsertCurrentLocation;

public static class InsertCurrentLocationCommandEndpoint
{
    public static RouteGroupBuilder InsertCurrentLocationGroupItemEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/",
                async (InsertCurrentLocationCommand command, IMediator mediator) =>
                    (await mediator.Send(command)).ToGenericResult())
            .WithName("InsertCurrentLocation")
            .MapToApiVersion(1, 0)
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
            .AddEndpointFilter<ValidationFilter<InsertCurrentLocationCommand>>();

        return group;
    }
}