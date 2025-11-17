using NakliyeTakip.Shared;

namespace NakliyeTakip.Location.API.Features.Location.InsertCurrentLocation;
public record InsertCurrentLocationCommand(double Longitude, double Latitude, Guid UserId) : IRequestByServiceResult<Guid>;