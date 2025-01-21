using FluentAssertions;
using System;
using Xunit;

namespace H.Necessaire.Testicles.Unit
{
    public class PartialPeriodOfTimeScenarios
    {
        [Fact(DisplayName = "PartialPeriodOfTime Duration For Infinites Works As Expected")]
        public void PartialPeriodOfTime_Duration_For_Infinites_Works_As_Expected()
        {
            DateTime now = DateTime.UtcNow;
            PartialPeriodOfTime timeless = (null as PartialDateTime, null as PartialDateTime);
            PartialPeriodOfTime nowTillForever = (now as DateTime?, null as DateTime?);
            PartialPeriodOfTime foreverTillNow = (null as DateTime?, now as DateTime?);

            timeless.Duration.Should().BeNull(because: "Timeless period has no duration");
            nowTillForever.Duration.Should().BeNull(because: "Infinite period has no duration");
            foreverTillNow.Duration.Should().BeNull(because: "Infinite period has no duration");

            PartialPeriodOfTime precise = (now.AddDays(-15), now.AddDays(15));
            precise.Duration.Should().Be(TimeSpan.FromDays(30), because: "The period is precise");
        }

        [Fact(DisplayName = "PartialPeriodOfTime Duration For Single Part Same From and To Works As Expected")]
        public void PartialPeriodOfTime_Duration_For_Single_Part_Same_From_And_To_Works_As_Expected()
        {
            DateTime now = DateTime.UtcNow;

            PartialPeriodOfTime partial = new PartialDateTime { Year = now.Year };
            partial.Duration.Should().Be(new DateTime(now.Year, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc) - new DateTime(now.Year, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), because: "The full-year-only period is the full year");

            partial = new PartialDateTime { Month = now.Month };
            partial.Duration.Should().Be(new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month), 23, 59, 59, 999, DateTimeKind.Utc) - new DateTime(now.Year, now.Month, 1, 0, 0, 0, 0, DateTimeKind.Utc), because: "The full-month-only period is the full month of current year");

            partial = new PartialDateTime { DayOfMonth = now.Day };
            partial.Duration.Should().Be(new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999, DateTimeKind.Utc) - new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0, DateTimeKind.Utc), because: "The full-day-only period is the full day of current month of current year");

            partial = new PartialDateTime { Hour = now.Hour };
            partial.Duration.Should().Be(new DateTime(now.Year, now.Month, now.Day, now.Hour, 59, 59, 999, DateTimeKind.Utc) - new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, 0, DateTimeKind.Utc), because: "The full-hour-only period is the full hour of current date");

            partial = new PartialDateTime { Minute = now.Minute };
            partial.Duration.Should().Be(new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 59, 999, DateTimeKind.Utc) - new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, 0, DateTimeKind.Utc), because: "The full-minute-only period is the full minute of current date of current hour");

            partial = new PartialDateTime { Second = now.Second };
            partial.Duration.Should().Be(new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, 999, DateTimeKind.Utc) - new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, 0, DateTimeKind.Utc), because: "The full-second-only period is the full second of current date of current hour of current minute");

            partial = new PartialDateTime { Millisecond = now.Millisecond };
            partial.Duration.Should().Be(new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond, DateTimeKind.Utc) - new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond, DateTimeKind.Utc), because: "The full-millisecond-only period is the full millisecond of current date of current hour of current minute of current second");
        }

        [Fact(DisplayName = "PartialPeriodOfTime Duration For Single Part Different From And To Works As Expected")]
        public void PartialPeriodOfTime_Duration_For_Single_Part_Different_From_And_To_Works_As_Expected()
        {
            DateTime now = DateTime.UtcNow;

            PartialPeriodOfTime partial = (new PartialDateTime { Year = now.Year }, new PartialDateTime { Year = now.Year + 1 });
            partial.Duration.Should().Be(new DateTime(now.Year + 1, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc) - new DateTime(now.Year, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), because: "The full-year-only period is the full year");

            partial = (new PartialDateTime { Month = 1 }, new PartialDateTime { Month = 6 });
            partial.Duration.Should().Be(new DateTime(now.Year, 6, DateTime.DaysInMonth(now.Year, 6), 23, 59, 59, 999, DateTimeKind.Utc) - new DateTime(now.Year, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), because: "The full-month-only period is the full month of current year");

            partial = (new PartialDateTime { DayOfMonth = 1 }, new PartialDateTime { DayOfMonth = 15 });
            partial.Duration.Should().Be(new DateTime(now.Year, now.Month, 15, 23, 59, 59, 999, DateTimeKind.Utc) - new DateTime(now.Year, now.Month, 1, 0, 0, 0, 0, DateTimeKind.Utc), because: "The full-day-only period is the full day of current month of current year");

            partial = (new PartialDateTime { Hour = 0 }, new PartialDateTime { Hour = 12 });
            partial.Duration.Should().Be(new DateTime(now.Year, now.Month, now.Day, 12, 59, 59, 999, DateTimeKind.Utc) - new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0, DateTimeKind.Utc), because: "The full-hour-only period is the full hour of current date");

            partial = (new PartialDateTime { Minute = 0 }, new PartialDateTime { Minute = 30 });
            partial.Duration.Should().Be(new DateTime(now.Year, now.Month, now.Day, now.Hour, 30, 59, 999, DateTimeKind.Utc) - new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, 0, DateTimeKind.Utc), because: "The full-minute-only period is the full minute of current date of current hour");

            partial = (new PartialDateTime { Second = 0 }, new PartialDateTime { Second = 45 });
            partial.Duration.Should().Be(new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 45, 999, DateTimeKind.Utc) - new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, 0, DateTimeKind.Utc), because: "The full-second-only period is the full second of current date of current hour of current minute");

            partial = (new PartialDateTime { Millisecond = 0 }, new PartialDateTime { Millisecond = 500 });
            partial.Duration.Should().Be(new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, 500, DateTimeKind.Utc) - new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, 0, DateTimeKind.Utc), because: "The full-millisecond-only period is the full millisecond of current date of current hour of current minute of current second");
        }

        [Fact(DisplayName = "PartialPeriodOfTime Duration For Multiple Parts Various From And To Works As Expected")]
        public void PartialPeriodOfTime_Duration_For_Multiple_Parts_Various_From_And_To_Works_As_Expected()
        {
            DateTime now = DateTime.UtcNow;

            PartialPeriodOfTime partial = (new PartialDateTime { Year = now.Year, Month = 1 }, new PartialDateTime { Year = now.Year, Month = 6 });
            partial.Duration.Should().Be(new DateTime(now.Year, 6, DateTime.DaysInMonth(now.Year, 6), 23, 59, 59, 999, DateTimeKind.Utc) - new DateTime(now.Year, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), because: "The year-month period is the full months from start of day to end of day");

            partial = (new PartialDateTime { Month = 3, DayOfMonth = 15 }, new PartialDateTime { Month = 5, DayOfMonth = 5 });
            partial.Duration.Should().Be(new DateTime(now.Year, 5, 5, 23, 59, 59, 999, DateTimeKind.Utc) - new DateTime(now.Year, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), because: "The month-day period is the full days from start of day to end of day");

            partial = (new PartialDateTime { Month = 3, DayOfMonth = 15, Year = now.Year }, new PartialDateTime { Month = 5, DayOfMonth = 5, Year = now.Year + 2 });
            partial.Duration.Should().Be(new DateTime(now.Year + 2, 5, 5, 23, 59, 59, 999, DateTimeKind.Utc) - new DateTime(now.Year, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), because: "The year-month-day period is the full days from start of day to end of day");

            partial = (new PartialDateTime { DayOfMonth = 1, Hour = 12 }, new PartialDateTime { DayOfMonth = 15, Hour = 12 });
            partial.Duration.Should().Be(new DateTime(now.Year, now.Month, 15, 12, 59, 59, 999, DateTimeKind.Utc) - new DateTime(now.Year, now.Month, 1, 12, 0, 0, 0, DateTimeKind.Utc), because: "The day-hour period is the full hours from start of day to end of day");

            partial = (new PartialDateTime { Hour = 8, Minute = 15 }, new PartialDateTime { Hour = 18, Minute = 30 });
            partial.Duration.Should().Be(new DateTime(now.Year, now.Month, now.Day, 18, 30, 59, 999, DateTimeKind.Utc) - new DateTime(now.Year, now.Month, now.Day, 8, 15, 0, 0, DateTimeKind.Utc), because: "The hour-minute period is the full minutes from start of hour to end of hour");

            partial = (new PartialDateTime { Minute = 3, Second = 12 }, new PartialDateTime { Minute = 17, Second = 42 });
            partial.Duration.Should().Be(new DateTime(now.Year, now.Month, now.Day, now.Hour, 17, 42, 999, DateTimeKind.Utc) - new DateTime(now.Year, now.Month, now.Day, now.Hour, 3, 12, 0, DateTimeKind.Utc), because: "The minute-second period is the full seconds from start of minute to end of minute");

            partial = (new PartialDateTime { Second = 1, Millisecond = 123 }, new PartialDateTime { Second = 17, Millisecond = 456 });
            partial.Duration.Should().Be(new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 17, 456, DateTimeKind.Utc) - new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 1, 123, DateTimeKind.Utc), because: "The second-millisecond period is the full milliseconds from start of second to end of second");

            partial = (new PartialDateTime { Month = 3, DayOfMonth = 15, Hour = 8, Minute = 5 }, new PartialDateTime { Month = 5, DayOfMonth = 5, Hour = 17, Minute = 35 });
            partial.Duration.Should().Be(new DateTime(now.Year, 5, 5, 17, 35, 59, 999, DateTimeKind.Utc) - new DateTime(now.Year, 3, 15, 8, 5, 0, 0, DateTimeKind.Utc), because: "The month-day-hour-minute period is the full minutes from start of hour to end of hour");

            partial = (new PartialDateTime { Month = 1, DayOfMonth = 15 }, new PartialDateTime { Month = 2, DayOfMonth = 14, Hour = 21, Minute = 35, Second = 0, Millisecond = 0 });
            partial.Duration.Should().Be(new DateTime(now.Year, 2, 14, 21, 35, 0, 0, DateTimeKind.Utc) - new DateTime(now.Year, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), because: "The month-day-hour-minute-second-millisecond period is the full milliseconds from start of second to end of second");
        }
    }
}
