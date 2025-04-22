using FluentAssertions;
using System;
using Xunit;

namespace H.Necessaire.Testicles.Unit
{
    public class PeriodOfTimeScenarios
    {
        [Fact(DisplayName = "PeriodOfTime Infinities Work As Expected")]
        public void PeriodOfTime_Infinities_Work_As_Expected()
        {
            PeriodOfTime periodOfTime = new PeriodOfTime();

            periodOfTime.IsInfinite.Should().BeTrue(because: "The default Period of time is infinite, has no beginning or end");

            periodOfTime = (DateTime.Today, null as DateTime?);
            periodOfTime.IsInfinite.Should().BeTrue(because: "The Period of time is infinite, as it has no end");
            periodOfTime.IsUntilForever.Should().BeTrue(because: "The Period of time is until forever, as it has no end");
            periodOfTime.IsSinceForever.Should().BeFalse(because: "The Period of time is not since forever, as it has a beginning");

            periodOfTime = (null as DateTime?, DateTime.Today);
            periodOfTime.IsInfinite.Should().BeTrue(because: "The Period of time is infinite, as it has no beginning");
            periodOfTime.IsUntilForever.Should().BeFalse(because: "The Period of time is not until forever, as it has an end");
            periodOfTime.IsSinceForever.Should().BeTrue(because: "The Period of time is since forever, as it has no beginning");

            DateTime now = DateTime.UtcNow;
            periodOfTime = (now.AddDays(-15), now.AddDays(15));
            periodOfTime.IsInfinite.Should().BeFalse(because: "The Period of time is well defined, has both beginning and end");
            periodOfTime.IsUntilForever.Should().BeFalse(because: "The Period of time is not until forever, as it has an end");
            periodOfTime.IsSinceForever.Should().BeFalse(because: "The Period of time is not since forever, as it has a beginning");
        }

        [Fact(DisplayName = "PeriodOfTime Active Or Not Work As Expected")]
        public void PeriodOfTime_Active_Or_Not_Work_As_Expected()
        {
            DateTime now = DateTime.UtcNow;

            PeriodOfTime periodOfTime = new PeriodOfTime();
            periodOfTime.HasEnded(asOf: now).Should().BeFalse(because: "The default Period of time is infinite, has no beginning or end, so it never ends");
            periodOfTime.HasStarted(asOf: now).Should().BeTrue(because: "The default Period of time is infinite, has no beginning or end, so it's started since forever");
            periodOfTime.IsActive(asOf: now).Should().BeTrue(because: "The default Period of time is infinite, has no beginning or end, so it's active since forever and until forever");
            periodOfTime.IsInactive(asOf: now).Should().BeFalse(because: "The default Period of time is infinite, has no beginning or end, so it's never inactive");

            periodOfTime = (now.AddDays(-15), null as DateTime?);
            periodOfTime.HasEnded(asOf: now).Should().BeFalse(because: "The Period of time has no end, so it never ends");
            periodOfTime.HasStarted(asOf: now).Should().BeTrue(because: "The Period of time has started prior to now");
            periodOfTime.HasStarted(asOf: now.AddDays(-16)).Should().BeFalse(because: "The Period of time has not yet started as of given date");
            periodOfTime.IsActive(asOf: now).Should().BeTrue(because: "The Period of time has started prior to now and it never ends, so it's active");
            periodOfTime.IsActive(asOf: now.AddDays(-16)).Should().BeFalse(because: "The Period of time has not yet started as of given date");
            periodOfTime.IsInactive(asOf: now).Should().BeFalse(because: "The Period of time has started prior to now and it never ends, so it's not inactive");
            periodOfTime.IsInactive(asOf: now.AddDays(-16)).Should().BeTrue(because: "The Period of time has not yet started as of given date, so it's inactive");

            periodOfTime = (null as DateTime?, now.AddDays(15));
            periodOfTime.HasStarted(asOf: now).Should().BeTrue(because: "The Period of time has no beginning, so it's always started'");
            periodOfTime.HasEnded(asOf: now).Should().BeFalse(because: "The Period of time has not yet ended as of now");
            periodOfTime.HasEnded(asOf: now.AddDays(16)).Should().BeTrue(because: "The Period of time has ended as of given date");
            periodOfTime.IsActive(asOf: now).Should().BeTrue(because: "The Period of time has started prior to now and it didn't yet end as of now");
            periodOfTime.IsActive(asOf: now.AddDays(16)).Should().BeFalse(because: "The Period of time has ended as of given date");
            periodOfTime.IsInactive(asOf: now).Should().BeFalse(because: "The Period of time has not ended as of now, so it's not inactive");
            periodOfTime.IsInactive(asOf: now.AddDays(16)).Should().BeTrue(because: "The Period of time has ended as of given date, so it's inactive");

            periodOfTime = (now.AddDays(-15), now.AddDays(15));
            periodOfTime.HasStarted(asOf: now).Should().BeTrue(because: "The Period of time has started as of now");
            periodOfTime.HasStarted(asOf: now.AddDays(-16)).Should().BeFalse(because: "The Period of time has not started as of given date");
            periodOfTime.HasStarted(asOf: now.AddDays(16)).Should().BeTrue(because: "The Period of time has started as of given date, even though it has also ended");
            periodOfTime.HasEnded(asOf: now).Should().BeFalse(because: "The Period of time has not ended as of now");
            periodOfTime.HasEnded(asOf: now.AddDays(-16)).Should().BeFalse(because: "The Period of time has not ended as of given date, event though it has not even started");
            periodOfTime.HasEnded(asOf: now.AddDays(16)).Should().BeTrue(because: "The Period of time has ended as of given date");
        }

        [Fact(DisplayName = "PeriodOfTime Overlappings Work As Expected")]
        public void PeriodOfTime_Overlappings_Work_As_Expected()
        {
            DateTime now = DateTime.UtcNow;

            PeriodOfTime a = (null, null);
            PeriodOfTime b = (null, null);
            a.IsOverlapping(b).Should().BeTrue(because: "Both periods are timeless, so they overlap");
            a.IsCompletelyBefore(b).Should().BeFalse(because: "Both periods are timeless");
            a.IsBeforeOrIntersects(b).Should().BeTrue(because: "Both periods are timeless, so they interesect");
            a.IsCompletelyAfter(b).Should().BeFalse(because: "Both periods are timeless");
            a.IsAfterOrIntersects(b).Should().BeTrue(because: "Both periods are timeless, so they interesect");
            a.Intersect(b).IsTimeless.Should().BeTrue(because: "Both periods are timeless, so their intersection is infinite");

            a = (null as DateTime?, now);
            a.IsOverlapping(b).Should().BeTrue(because: "a is infinite, b is timeless, so they overlap");
            a.IsCompletelyBefore(b).Should().BeFalse(because: "a is (-∞, now], b is (-∞, ∞)");
            a.Intersect(b).Should().Be(a, because: "b is timeless");
            a.Unite(b).Should().Be(b, because: "b is timeless");
        }
    }
}
