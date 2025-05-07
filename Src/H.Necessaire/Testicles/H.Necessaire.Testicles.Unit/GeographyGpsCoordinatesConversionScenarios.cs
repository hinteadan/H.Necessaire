using FluentAssertions;
using Xunit;

namespace H.Necessaire.Testicles.Unit
{
    public class GeographyGpsCoordinatesConversionScenarios
    {
        /*
         * According to Google Maps
         * ========================
         * 
         * 46°27'32.8"N 23°33'10.6"E == 46.459110, 23.552939
         * 47°41'10.0"N 112°41'27.6"W == 47.686120, -112.690994
         * 33°10'07.3"S 67°08'57.8"W == -33.168682, -67.149395
         * 33°55'05.3"S 141°16'06.3"E == -33.918148, 141.268403
         * 
         * */

        [Fact(DisplayName = "GeographyGpsCoordinates are correctly being converted from Decimal to DMS")]
        public void GeographyGpsCoordinates_Decimal_To_DMS()
        {
            GpsPoint gpsPoint = (46.459110, 23.552939);
            GeoDmsCoordinates dmsPoint = gpsPoint;
            GeoDmsCoordinates expectedDmsPoint = ((46, 27, 32.8, GeoDmsLatDirection.North), (23, 33, 10.6, GeoDmsLngDirection.East));
            dmsPoint.Should().Be(expectedDmsPoint, because: "that's the conversion Google Maps gives for NE hemisphere");

            gpsPoint = (47.686120, -112.690994);
            dmsPoint = gpsPoint;
            expectedDmsPoint = ((47, 41, 10.0, GeoDmsLatDirection.North), (112, 41, 27.6, GeoDmsLngDirection.West));
            dmsPoint.Should().Be(expectedDmsPoint, because: "that's the conversion Google Maps gives for NW hemisphere");

            gpsPoint = (-33.168682, -67.149395);
            dmsPoint = gpsPoint;
            expectedDmsPoint = ((33, 10, 7.3, GeoDmsLatDirection.South), (67, 8, 57.8, GeoDmsLngDirection.West));
            dmsPoint.Should().Be(expectedDmsPoint, because: "that's the conversion Google Maps gives for SW hemisphere");

            gpsPoint = (-33.918148, 141.268403);
            dmsPoint = gpsPoint;
            expectedDmsPoint = ((33, 55, 5.3, GeoDmsLatDirection.South), (141, 16, 6.3, GeoDmsLngDirection.East));
            dmsPoint.Should().Be(expectedDmsPoint, because: "that's the conversion Google Maps gives for SE hemisphere");
        }

        [Fact(DisplayName = "GeographyGpsCoordinates are correctly being converted from DMS to Decimal")]
        public void GeographyGpsCoordinate_DMS_To_Decimal()
        {

        }

        [Fact(DisplayName = "ToDMS(this double degrees, out int deg, out int min, out double sec) works as expected")]
        public void DegreesToDMS_Extension_Method_Works_as_Expected()
        {
            0.000000.ToDMS(out var deg, out var min, out var sec); 
            deg.Should().Be(0, because: "0.000000° has 0°");
            min.Should().Be(0, because: "0.000000 has 0'");
            sec.Should().Be(0, because: "0.000000 has 0\"");

            45.000000.ToDMS(out deg, out min, out sec);
            deg.Should().Be(45, because: "45.000000° has 45°");
            min.Should().Be(0, because: "45.000000° has 0'");
            sec.Should().Be(0, because: "45.000000° has 0\"");

            15.125000.ToDMS(out deg, out min, out sec);
            deg.Should().Be(15, because: "15.125000° has 15°");
            min.Should().Be(7, because: "15.125000° has 7'");
            sec.Should().Be(30.0000, because: "15.125000° has 30.0000\"");

            46.459110.ToDMS(out deg, out min, out sec);
            deg.Should().Be(46, because: "46.459110° has 46°");
            min.Should().Be(27, because: "46.459110° has 27'");
            sec.Should().BeApproximately(32.8000, 1, because: "46.459110° has 32.8000\"");
        }

        [Fact(DisplayName = "ToDegrees(int deg, int min, double sec) works as expected")]
        public void DMSToDegress_Extension_Method_Works_as_Expected()
        {
            
        }
    }
}
