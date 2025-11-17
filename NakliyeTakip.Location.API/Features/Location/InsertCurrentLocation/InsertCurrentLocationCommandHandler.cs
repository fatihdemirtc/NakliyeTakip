using AutoMapper;
using MassTransit;
using MediatR;
using MongoDB.Driver.Core.Misc;
using NakliyeTakip.Location.API.Repositories;
using NakliyeTakip.Shared;

namespace NakliyeTakip.Location.API.Features.Location.InsertCurrentLocation;

public class InsertCurrentLocationCommandHandler(AppDbContext context, IMapper mapper) : IRequestHandler<InsertCurrentLocationCommand, ServiceResult<Guid>>
{
    public async Task<ServiceResult<Guid>> Handle(InsertCurrentLocationCommand request, CancellationToken cancellationToken)
    {
        
        var newLocation = mapper.Map<LocationEntity>(request);
        newLocation.Created = DateTime.Now;
        newLocation.Id = NewId.NextSequentialGuid(); // index performance
        newLocation.Longitude = request.Longitude;
        newLocation.Latitude = request.Latitude;

        context.Locations.Add(newLocation);
        await context.SaveChangesAsync(cancellationToken);

        return ServiceResult<Guid>.SuccessAsCreated(newLocation.Id, $"/api/locations/{newLocation.Id}");
    }
}
