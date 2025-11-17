using AutoMapper;
using MongoDB.Driver.Core.Misc;
using NakliyeTakip.Location.API.Features.Location.InsertCurrentLocation;

namespace NakliyeTakip.Location.API.Features.Location
{
    public class LocationMapping : Profile
    {
        public LocationMapping()
        {
            CreateMap<InsertCurrentLocationCommand, LocationEntity>();
        }
    }
}
