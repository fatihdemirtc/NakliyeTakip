using MongoDB.Bson.Serialization.Attributes;

namespace NakliyeTakip.Location.API.Repositories
{
    public class BaseEntity
    {
        [BsonElement("_id")]
        public Guid Id { get; set; }
    }
}
