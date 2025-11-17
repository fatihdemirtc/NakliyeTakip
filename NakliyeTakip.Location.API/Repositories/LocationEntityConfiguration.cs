using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;
using NakliyeTakip.Location.API.Features.Location;

namespace NakliyeTakip.Location.API.Repositories
{
    public class LocationEntityConfiguration : IEntityTypeConfiguration<LocationEntity>
    {
        public void Configure(EntityTypeBuilder<LocationEntity> builder)
        {
            builder.ToCollection("locations");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Longitude).HasElementName("longitude");
            builder.Property(x => x.Latitude).HasElementName("latitude");
            builder.Property(x => x.Created).HasElementName("created");
            builder.Property(x => x.UserId).HasElementName("userId");
        }
    }
}
