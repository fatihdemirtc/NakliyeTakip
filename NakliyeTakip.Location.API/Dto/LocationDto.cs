namespace NakliyeTakip.Location.API.Dto
{
    public record LocationDto
    {
        public DateTime LastSeen { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
