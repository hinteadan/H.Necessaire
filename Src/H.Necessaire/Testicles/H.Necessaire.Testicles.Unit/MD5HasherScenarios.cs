using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace H.Necessaire.Testicles.Unit
{
    public class MD5HasherScenarios
    {
        [Fact(DisplayName = "MD5Hasher Correctly Hashes A String Value")]
        public async Task MD5Hasher_Correctly_Hashes_A_String_Value()
        {
            MD5Hasher hasher = new MD5Hasher();

            (await hasher.Hash("hinteadan@yahoo.co.uk")).Hash.Should().Be("1f82f01fafd99266dd19cb871935eb51", because: "This is the calculated hash for the given value");
            (await hasher.VerifyMatch("hinteadan@yahoo.co.uk", new SecuredHash { Hash = "1f82f01fafd99266dd19cb871935eb51" })).IsSuccessful.Should().BeTrue(because: "The hashes should match");
            (await hasher.VerifyMatch("hintea_dan@yahoo.co.uk", new SecuredHash { Hash = "1f82f01fafd99266dd19cb871935eb51" })).IsSuccessful.Should().BeFalse(because: "The hashes should NOT match");
        }
    }
}
