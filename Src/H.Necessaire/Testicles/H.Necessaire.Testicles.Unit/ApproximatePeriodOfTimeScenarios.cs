using FluentAssertions;
using System;
using Xunit;

namespace H.Necessaire.Testicles.Unit
{
    public class ApproximatePeriodOfTimeScenarios
    {
        [Fact(DisplayName = "ApproximatePeriodOfTime Start Time Operations Work As Expcted")]
        public void ApproximatePeriodOfTime_Start_Time_Operations_Work_As_Expcted()
        {
            DateTime now = DateTime.UtcNow;

            ApproximatePeriodOfTime period = (null as PartialPeriodOfTime, null as PartialPeriodOfTime);
            period.ClosestStartTime(asOf: now).Should().BeNull(because: "period is timeless");
            period.FurthestStartTime(asOf: now).Should().BeNull(because: "period is timeless");

            period = ((now.AddDays(-10), now.AddDays(11)), null);
            period.ClosestStartTime(asOf: now).Should().BeCloseTo(now.AddDays(-10), TimeSpan.FromMilliseconds(1), because: "start period From is closest as of now");
            period.FurthestStartTime(asOf: now).Should().BeCloseTo(now.AddDays(11), TimeSpan.FromMilliseconds(1), because: "start period To is furthest as of now");
        }

        [Fact(DisplayName = "ApproximatePeriodOfTime End Time Operations Work As Expcted")]
        public void ApproximatePeriodOfTime_End_Time_Operations_Work_As_Expcted()
        {
            DateTime now = DateTime.UtcNow;

            ApproximatePeriodOfTime period = (null as PartialPeriodOfTime, null as PartialPeriodOfTime);
            period.ClosestEndTime(asOf: now).Should().BeNull(because: "period is timeless");
            period.FurthestEndTime(asOf: now).Should().BeNull(because: "period is timeless");

            period = (null, (now.AddDays(15), now.AddDays(20)));
            period.ClosestEndTime(asOf: now).Should().Be(now.AddDays(15).WithoutMicroseconds(), because: "end period From is closest as of now");
            period.FurthestEndTime(asOf: now).Should().Be(now.AddDays(20).WithoutMicroseconds(), because: "end period To is furthest as of now");
        }

        [Fact(DisplayName = "ApproximatePeriodOfTime Period Of Time Operations Work As Expcted")]
        public void ApproximatePeriodOfTime_Period_Of_Time_Operations_Work_As_Expcted()
        {
            DateTime now = DateTime.UtcNow;

            ApproximatePeriodOfTime period = (null as PartialPeriodOfTime, null as PartialPeriodOfTime);
            period.ToMinimumPeriodOfTime().IsTimeless.Should().BeTrue(because: "period is timeless");
            period.ToMaximumPeriodOfTime().IsTimeless.Should().BeTrue(because: "period is timeless");

            period = ((now.AddDays(-15), now.AddDays(-3)), null);
            period.ToMinimumPeriodOfTime().Should().Be((now.AddDays(-3).WithoutMicroseconds(), null as DateTime?));
            period.ToMaximumPeriodOfTime().Should().Be((now.AddDays(-15).WithoutMicroseconds(), null as DateTime?));

            period = (null, (now.AddDays(15), now.AddDays(20)));
            period.ToMinimumPeriodOfTime().Should().Be((null as DateTime ?, now.AddDays(15).WithoutMicroseconds()));
            period.ToMaximumPeriodOfTime().Should().Be((null as DateTime?, now.AddDays(20).WithoutMicroseconds()));

            period = ((now.AddDays(-15), now.AddDays(-3)), (now.AddDays(15), now.AddDays(20)));
            period.ToMinimumPeriodOfTime().Should().Be((now.AddDays(-3).WithoutMicroseconds(), now.AddDays(15).WithoutMicroseconds()));
            period.ToMaximumPeriodOfTime().Should().Be((now.AddDays(-15).WithoutMicroseconds(), now.AddDays(20).WithoutMicroseconds()));
        }

        [Fact(DisplayName = "ApproximatePeriodOfTime Durations Work As Expcted")]
        public void ApproximatePeriodOfTime_Durations_Work_As_Expcted()
        {
            DateTime now = DateTime.UtcNow;

            ApproximatePeriodOfTime period = (null as PartialPeriodOfTime, null as PartialPeriodOfTime);
            period.MinimumDuration.Should().BeNull(because: "period is timeless");
            period.MaximumDuration.Should().BeNull(because: "period is timeless");
            period.AverageDuration.Should().BeNull(because: "period is timeless");

            period = ((now.AddDays(-15), now.AddDays(-3)), null);
            period.MinimumDuration.Should().BeNull(because: "period is until forever");
            period.MaximumDuration.Should().BeNull(because: "period is until forever");
            period.AverageDuration.Should().BeNull(because: "period is until forever");

            period = (null, (now.AddDays(15), now.AddDays(20)));
            period.MinimumDuration.Should().BeNull(because: "period is since forever");
            period.MaximumDuration.Should().BeNull(because: "period is since forever");
            period.AverageDuration.Should().BeNull(because: "period is since forever");

            period = ((now.AddDays(-15), now.AddDays(-3)), (now.AddDays(15), now.AddDays(20)));
            period.MinimumDuration.Should().Be(now.AddDays(15) - now.AddDays(-3));
            period.MaximumDuration.Should().Be(now.AddDays(20) - now.AddDays(-15));
            period.AverageDuration.Should().Be(TimeSpan.FromTicks((now.AddDays(15) - now.AddDays(-3)).Ticks + (now.AddDays(20) - now.AddDays(-15)).Ticks) / 2);
        }
    }
}
