namespace H.Necessaire
{
    public class GeoLocation
    {
        public GeoAddress Address { get; set; }
        public GpsPoint? GpsPosition { get; set; }
        public GpsArea GpsArea { get; set; }

        /// <summary>
        /// ID - Time Zone Name
        /// Value - TimeZoneDifferenceToUtcInMinutes
        /// </summary>
        public Note[] TimeZones { get; set; }
        public Note[] DialCodes { get; set; }
        public Note[] Languages { get; set; }
        public Note[] Currencies { get; set; }
    }
}
