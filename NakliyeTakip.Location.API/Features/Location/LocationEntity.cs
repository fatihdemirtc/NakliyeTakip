using NakliyeTakip.Location.API.Repositories;

namespace NakliyeTakip.Location.API.Features.Location
{
    public class LocationEntity : BaseEntity
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime Created { get; set; }
        public Guid UserId { get; set; }
    }
}
