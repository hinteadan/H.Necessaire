using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace H.Necessaire.Testicles.Unit
{
    public class VersionScenarios
    {
        const string nullVersionNumberLabel = "<NULL Version Number>";

        [Fact(DisplayName = "VersionNumber Correctly Compares For Equality And Higher Or Lower")]
        public void VersionNumber_Correctly_Compares_For_Equality_And_Higher_Or_Lower()
        {
            VersionNumber nullVersion = null;
            VersionNumber unknownVersion = VersionNumber.Unknown;
            VersionNumber versionA = new VersionNumber(major: 1, minor: 0, patch: 0, build: null, suffix: null);
            VersionNumber versionB = new VersionNumber(major: 2, minor: 17, patch: 1, build: 20122, suffix: "beta");
            VersionNumber versionC = new VersionNumber(major: 2, minor: 17, patch: 1, build: 20123, suffix: "beta2");
            VersionNumber versionD = new VersionNumber(major: 2, minor: 18, patch: null, build: null, suffix: null);
            VersionNumber versionE = new VersionNumber(major: 3, minor: 0, patch: null, build: null, suffix: null);

            AssertSameReferenceComparisonsFor(nullVersion, unknownVersion, versionA, versionB, versionC, versionD, versionE);

            AssertHigherThanComparisonsFor(
                (unknownVersion, nullVersion)

                , (versionA, unknownVersion)
                , (versionA, nullVersion)

                , (versionB, unknownVersion)
                , (versionB, nullVersion)
                , (versionB, versionA)

                , (versionC, unknownVersion)
                , (versionC, nullVersion)
                , (versionC, versionA)
                , (versionC, versionB)

                , (versionD, unknownVersion)
                , (versionD, nullVersion)
                , (versionD, versionA)
                , (versionD, versionB)
                , (versionD, versionC)

                , (versionE, unknownVersion)
                , (versionE, nullVersion)
                , (versionE, versionA)
                , (versionE, versionB)
                , (versionE, versionC)
                , (versionE, versionD)
            );

            AssertLowerThanComparisonsFor(
                (nullVersion, unknownVersion)

                , (unknownVersion, versionA)
                , (nullVersion, versionA)

                , (unknownVersion, versionB)
                , (nullVersion, versionB)
                , (versionA, versionB)

                , (unknownVersion, versionC)
                , (nullVersion, versionC)
                , (versionA, versionC)
                , (versionB, versionC)

                , (unknownVersion, versionD)
                , (nullVersion, versionD)
                , (versionA, versionD)
                , (versionB, versionD)
                , (versionC, versionD)

                , (unknownVersion, versionE)
                , (nullVersion, versionE)
                , (versionA, versionE)
                , (versionB, versionE)
                , (versionC, versionE)
                , (versionD, versionE)
            );
        }

        private void AssertSameReferenceComparisonsFor(params VersionNumber[] versionNumbers)
        {
            if (versionNumbers?.Any() != true)
                return;

            foreach (VersionNumber versionNumber in versionNumbers)
            {
                AssertSameReferenceComparisons(versionNumber);
            }
        }
        private void AssertHigherThanComparisonsFor(params (VersionNumber, VersionNumber)[] pairs)
        {
            if (pairs?.Any() != true)
                return;

            foreach ((VersionNumber, VersionNumber) tuple in pairs)
            {
                AssertHigherThanComparisons(tuple.Item1, tuple.Item2);
            }
        }
        private void AssertLowerThanComparisonsFor(params (VersionNumber, VersionNumber)[] pairs)
        {
            if (pairs?.Any() != true)
                return;

            foreach ((VersionNumber, VersionNumber) tuple in pairs)
            {
                AssertLowerThanComparisons(tuple.Item1, tuple.Item2);
            }
        }

        private void AssertSameReferenceComparisons(VersionNumber versionNumber)
        {
            string label = versionNumber is null ? nullVersionNumberLabel : versionNumber.ToString();

#pragma warning disable CS1718 // Comparison made to same variable
            (versionNumber == versionNumber).Should().BeTrue(because: $"{label} == {label}");
            (versionNumber != versionNumber).Should().BeFalse(because: $"{label} == {label}");
            if (versionNumber is null)
                new Action(() => versionNumber.Equals(versionNumber)).Should().Throw<NullReferenceException>(because: $"{label} is null, can't call Equals() on null");
            else
                versionNumber.Equals(versionNumber).Should().BeTrue(because: $"{label} Equals {label}");

            (versionNumber > versionNumber).Should().BeFalse(because: $"{label} is not > {label}");
            (versionNumber < versionNumber).Should().BeFalse(because: $"{label} is not < {label}");
            (versionNumber >= versionNumber).Should().BeTrue(because: $"{label} is >= (equal to) {label}");
            (versionNumber <= versionNumber).Should().BeTrue(because: $"{label} is <= (equal to) {label}");
#pragma warning restore CS1718 // Comparison made to same variable

            if (versionNumber is null)
                new Action(() => versionNumber.CompareTo(versionNumber)).Should().Throw<NullReferenceException>(because: $"{label} is null, can't call CompareTo() on null");
            else
                versionNumber.CompareTo(versionNumber).Should().Be(0, because: $"{label} CompareTo {label} is equal, therefore Zero");
        }
        private void AssertHigherThanComparisons(VersionNumber a, VersionNumber b)
        {
            string labelA = a is null ? nullVersionNumberLabel : a.ToString();
            string labelB = b is null ? nullVersionNumberLabel : b.ToString();

            (a == b).Should().BeFalse(because: $"{labelA} != {labelB}");
            (a != b).Should().BeTrue(because: $"{labelA} != {labelB}");
            if (a is null)
                new Action(() => a.Equals(b)).Should().Throw<NullReferenceException>(because: $"{labelA} is null, can't call Equals() on null");
            else
                a.Equals(b).Should().BeFalse(because: $"{labelA} Does Not Equal {labelB}");

            (a > b).Should().BeTrue(because: $"{labelA} is > {labelB}");
            (a < b).Should().BeFalse(because: $"{labelA} is not < {labelB}");
            (a >= b).Should().BeTrue(because: $"{labelA} is >= (higher than) {labelB}");
            (a <= b).Should().BeFalse(because: $"{labelA} is not <= {labelB}");

            if (a is null)
                new Action(() => a.CompareTo(b)).Should().Throw<NullReferenceException>(because: $"{labelA} is null, can't call CompareTo() on null");
            else
                a.CompareTo(b).Should().BeGreaterThan(0, because: $"{labelA} CompareTo {labelB} is higher");
        }
        private void AssertLowerThanComparisons(VersionNumber a, VersionNumber b)
        {
            string labelA = a is null ? nullVersionNumberLabel : a.ToString();
            string labelB = b is null ? nullVersionNumberLabel : b.ToString();

            (a == b).Should().BeFalse(because: $"{labelA} != {labelB}");
            (a != b).Should().BeTrue(because: $"{labelA} != {labelB}");
            if (a is null)
                new Action(() => a.Equals(b)).Should().Throw<NullReferenceException>(because: $"{labelA} is null, can't call Equals() on null");
            else
                a.Equals(b).Should().BeFalse(because: $"{labelA} Does Not Equal {labelB}");

            (a > b).Should().BeFalse(because: $"{labelA} is not > {labelB}");
            (a < b).Should().BeTrue(because: $"{labelA} is < {labelB}");
            (a >= b).Should().BeFalse(because: $"{labelA} is not >= {labelB}");
            (a <= b).Should().BeTrue(because: $"{labelA} is <= (lower than) {labelB}");

            if (a is null)
                new Action(() => a.CompareTo(b)).Should().Throw<NullReferenceException>(because: $"{labelA} is null, can't call CompareTo() on null");
            else
                a.CompareTo(b).Should().BeLessThan(0, because: $"{labelA} CompareTo {labelB} is lower");
        }
    }
}
