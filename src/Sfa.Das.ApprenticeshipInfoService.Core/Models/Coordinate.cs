namespace Sfa.Das.ApprenticeshipInfoService.Core.Models
{
    public sealed class Coordinate
    {
        public Coordinate() { }

        public Coordinate(double lat, double lon)
        {
            Lat = lat;
            Lon = lon;
        }

        public double Lat { get; set; }

        public double Lon { get; set; }

        public override string ToString()
        {
            return $"Longitude: {Lon}, Latitude: {Lat}";
        }
    }
}
