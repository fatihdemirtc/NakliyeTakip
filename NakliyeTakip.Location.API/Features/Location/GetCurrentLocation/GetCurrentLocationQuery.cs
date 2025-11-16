using NakliyeTakip.Location.API.Dto;
using NakliyeTakip.Shared;

namespace NakliyeTakip.Location.API.Features.Locations.GetCurrentLocation;

public record GetCurrentLocationQuery : IRequestByServiceResult<LocationDto>;

