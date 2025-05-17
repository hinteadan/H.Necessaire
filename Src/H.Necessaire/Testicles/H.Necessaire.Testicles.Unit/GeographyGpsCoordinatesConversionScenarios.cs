using FluentAssertions;
using System;
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
            AssertDmsCoordinates(dmsPoint, expectedDmsPoint, because: "that's the conversion Google Maps gives for NE hemisphere");

            gpsPoint = (47.686120, -112.690994);
            dmsPoint = gpsPoint;
            expectedDmsPoint = ((47, 41, 10.0, GeoDmsLatDirection.North), (112, 41, 27.6, GeoDmsLngDirection.West));
            AssertDmsCoordinates(dmsPoint, expectedDmsPoint, because: "that's the conversion Google Maps gives for NW hemisphere");

            gpsPoint = (-33.168682, -67.149395);
            dmsPoint = gpsPoint;
            expectedDmsPoint = ((33, 10, 7.3, GeoDmsLatDirection.South), (67, 8, 57.8, GeoDmsLngDirection.West));
            AssertDmsCoordinates(dmsPoint, expectedDmsPoint, because: "that's the conversion Google Maps gives for SW hemisphere");

            gpsPoint = (-33.918148, 141.268403);
            dmsPoint = gpsPoint;
            expectedDmsPoint = ((33, 55, 5.3, GeoDmsLatDirection.South), (141, 16, 6.3, GeoDmsLngDirection.East));
            AssertDmsCoordinates(dmsPoint, expectedDmsPoint, because: "that's the conversion Google Maps gives for SE hemisphere");
        }

        [Fact(DisplayName = "GeographyGpsCoordinates are correctly being converted from DMS to Decimal")]
        public void GeographyGpsCoordinate_DMS_To_Decimal()
        {
            GeoDmsCoordinates dmsPoint = ((46, 27, 32.8, GeoDmsLatDirection.North), (23, 33, 10.6, GeoDmsLngDirection.East));
            GpsPoint gpsPoint = dmsPoint;
            GpsPoint expectedGpsPoint = (46.459110, 23.552939);
            AssertGpsCoordinates(gpsPoint, expectedGpsPoint, because: "that's the conversion Google Maps gives for NE hemisphere");

            dmsPoint = ((47, 41, 10.0, GeoDmsLatDirection.North), (112, 41, 27.6, GeoDmsLngDirection.West));
            gpsPoint = dmsPoint;
            expectedGpsPoint = (47.686120, -112.690994);
            AssertGpsCoordinates(gpsPoint, expectedGpsPoint, because: "that's the conversion Google Maps gives for NW hemisphere");

            dmsPoint = ((33, 10, 7.3, GeoDmsLatDirection.South), (67, 8, 57.8, GeoDmsLngDirection.West));
            gpsPoint = dmsPoint;
            expectedGpsPoint = (-33.168682, -67.149395);
            AssertGpsCoordinates(gpsPoint, expectedGpsPoint, because: "that's the conversion Google Maps gives for SW hemisphere");

            dmsPoint = ((33, 55, 5.3, GeoDmsLatDirection.South), (141, 16, 6.3, GeoDmsLngDirection.East));
            gpsPoint = dmsPoint;
            expectedGpsPoint = (-33.918148, 141.268403);
            AssertGpsCoordinates(gpsPoint, expectedGpsPoint, because: "that's the conversion Google Maps gives for SE hemisphere");
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
            0.ToDegrees(0, 0.0).Should().BeApproximately(0.000000, 6, because: "0°0'0\" is 0.000000°");
            45.ToDegrees(0, 0.0).Should().BeApproximately(45.000000, 6, because: "45°0'0\" is 45.000000°");
            15.ToDegrees(7, 30.0000).Should().BeApproximately(15.125000, 6, because: "15°07'30.00\" is 15.125000°");
            46.ToDegrees(27, 32.8000).Should().BeApproximately(46.459110, 6, because: "46°27'32.80\" is 46.459110°");
        }

        static void AssertDmsCoordinates(GeoDmsCoordinates dmsPoint, GeoDmsCoordinates expectedDmsPoint, string because = null)
        {
            dmsPoint.Lat.Degrees.Should().Be(expectedDmsPoint.Lat.Degrees, because);
            dmsPoint.Lat.Minutes.Should().Be(expectedDmsPoint.Lat.Minutes, because);
            dmsPoint.Lat.Seconds.Should().BeApproximately(expectedDmsPoint.Lat.Seconds, 1, because);
            dmsPoint.Lat.Direction.Should().Be(expectedDmsPoint.Lat.Direction, because);

            dmsPoint.Lng.Degrees.Should().Be(expectedDmsPoint.Lng.Degrees, because);
            dmsPoint.Lng.Minutes.Should().Be(expectedDmsPoint.Lng.Minutes, because);
            dmsPoint.Lng.Seconds.Should().BeApproximately(expectedDmsPoint.Lng.Seconds, 1, because);
            dmsPoint.Lng.Direction.Should().Be(expectedDmsPoint.Lng.Direction, because);
        }

        static void AssertGpsCoordinates(GpsPoint gpsPoint, GpsPoint expectedGpsPoint, string because = null)
        {
            gpsPoint.LatInDegrees.Should().BeApproximately(expectedGpsPoint.LatInDegrees, 6, because);
            gpsPoint.LngInDegrees.Should().BeApproximately(expectedGpsPoint.LngInDegrees, 6, because);
        }
    }
}
