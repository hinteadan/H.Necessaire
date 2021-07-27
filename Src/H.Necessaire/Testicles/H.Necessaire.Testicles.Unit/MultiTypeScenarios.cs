using FluentAssertions;
using System;
using Xunit;

namespace H.Necessaire.Testicles.Unit
{
    public class MultiTypeScenarios
    {
        [Fact(DisplayName = "MultiType Can Be Composed By Any Two Types")]
        public void MultiType_Can_Be_Composed_By_Any_Two_Types()
        {
            MultiType<int, string> multiTypeA = 10;
            MultiType<int, string> multiTypeB = "hintee";

            multiTypeA.Read(readFirstType: x => x.Should().Be(10, "We assgined 10"));
            multiTypeB.Read(readSecondType: x => x.Should().Be("hintee", "We assgined 'hintee'"));

            multiTypeA.ToString().Should().Be("10");
            multiTypeB.ToString().Should().Be("hintee");
            multiTypeA.ToObject().Should().Be(10);
            multiTypeB.ToObject().Should().Be("hintee");
        }

        [Fact(DisplayName = "MultiType Compositions Work As Expected")]
        public void MultiType_Compositions_Work_As_Expected()
        {
            MultiType<int, MultiType<DateTime, TimeSpan>> composedMultitype = (MultiType<DateTime, TimeSpan>)TimeSpan.FromMilliseconds(10);

            composedMultitype.ToObject().Should().Be(TimeSpan.FromMilliseconds(10));
            composedMultitype.Read(readSecondType: x => x.Read(readSecondType: y => y.Should().Be(TimeSpan.FromMilliseconds(10))));

            composedMultitype = 17;

            composedMultitype.ToObject().Should().Be(17);
            composedMultitype.Read(readFirstType: x => x.Should().Be(17));
        }
    }
}
