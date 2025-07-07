using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace H.Necessaire.Testicles.Unit
{
    public class GpsPointScenarios
    {
        [Fact(DisplayName = "GpsPoint Distance In Meters Calcualtes As Expected")]
        public void GpsPoint_Distance_In_Meters_Calcualtes_As_Expected()
        {
            GpsPoint a = (46.750673036973524, 23.51814902463961);
            GpsPoint b = (46.753023363262145, 23.52817472089083);
            double approxDistanceExpected = 807;
            double calculatedDistance = a.ApproximateDistanceInMetersTo(b);

            calculatedDistance.Should().BeApproximately(approxDistanceExpected, 5);

            a = (46.745631786381416, 23.495941566942886);
            b = (46.78500658681929, 23.652726326564974);
            approxDistanceExpected = 12715;
            calculatedDistance = a.ApproximateDistanceInMetersTo(b);

            calculatedDistance.Should().BeApproximately(approxDistanceExpected, 5);

            a = (45.583061, 24.339465);
            b = (45.592258383808264, 25.02495472875307);
            approxDistanceExpected = 54.3 * 1000;
            calculatedDistance = a.ApproximateDistanceInMetersTo(b);

            calculatedDistance.Should().BeApproximately(approxDistanceExpected, 1000);
        }
    }
}
