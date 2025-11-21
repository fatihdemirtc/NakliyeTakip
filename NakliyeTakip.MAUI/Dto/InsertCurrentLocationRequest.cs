namespace NakliyeTakip.MAUI.Dto
{
    public record InsertCurrentLocationRequest
    {
        public Guid UserId{ get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
