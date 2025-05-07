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

        }

        [Fact(DisplayName = "GeographyGpsCoordinates are correctly being converted from DMS to Decimal")]
        public void GeographyGpsCoordinate_DMS_To_Decimal()
        {

        }
    }
}
