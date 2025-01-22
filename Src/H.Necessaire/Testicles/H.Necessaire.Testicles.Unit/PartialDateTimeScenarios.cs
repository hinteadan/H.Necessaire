using FluentAssertions;
using FluentAssertions.Extensions;
using System;
using Xunit;

namespace H.Necessaire.Testicles.Unit
{
    public class PartialDateTimeScenarios
    {
        [Fact(DisplayName = "PartialDateTime IsPrecise Works As Expected")]
        public void PartialDateTime_IsPrecise_Works_As_Expected()
        {
            new PartialDateTime().IsPrecise().Should().BeFalse(because: "A default PartialDateTime is not precise, it is infinite");
            DateTime.UtcNow.ToPartialDateTime().IsPrecise().Should().BeTrue(because: "A PartialDateTime created from a specific DateTime is precise");
            DateTime.UtcNow.ToDateOnlyPartialDateTime().IsPrecise().Should().BeFalse(because: "A PartialDateTime created from a specific DateTime but with date only is not precise");
            DateTime.UtcNow.ToTimeOnlyPartialDateTime().IsPrecise().Should().BeFalse(because: "A PartialDateTime created from a specific DateTime but with time only is not precise");
            DateTime.UtcNow.ToPartialDateTime(x => x.Year = null).IsPrecise().Should().BeFalse(because: "A PartialDateTime without year is not precise");
            DateTime.UtcNow.ToPartialDateTime(x => x.Month = null).IsPrecise().Should().BeFalse(because: "A PartialDateTime without month is not precise");
            DateTime.UtcNow.ToPartialDateTime(x => x.DayOfMonth = null).IsPrecise().Should().BeFalse(because: "A PartialDateTime without day of month is not precise");
            DateTime.UtcNow.ToPartialDateTime(x => x.Hour = null).IsPrecise().Should().BeFalse(because: "A PartialDateTime without hour is not precise");
            DateTime.UtcNow.ToPartialDateTime(x => x.Minute = null).IsPrecise().Should().BeFalse(because: "A PartialDateTime without minute is not precise");
            DateTime.UtcNow.ToPartialDateTime(x => x.Second = null).IsPrecise().Should().BeFalse(because: "A PartialDateTime without second is not precise");
            DateTime.UtcNow.ToPartialDateTime(x => x.Millisecond = null).IsPrecise().Should().BeTrue(because: "A PartialDateTime without millisecond is precise enough");
        }

        [Fact(DisplayName = "PartialDateTime Equality Works As Expected")]
        public void PartialDateTime_Equality_Works_As_Expected()
        {
            new PartialDateTime().Should().Be(new PartialDateTime(), because: "Two default PartialDateTime instances should be equal");

            DateTime now = DateTime.UtcNow;
            now.ToPartialDateTime().Should().Be(now.ToPartialDateTime(), because: "Two PartialDateTime instances created from the same DateTime should be equal");
            now.ToDateOnlyPartialDateTime().Should().NotBe(now.ToTimeOnlyPartialDateTime(), because: "The partial date times are different");

            now.ToPartialDateTime(x => x.Year = null).Should().Be(now.ToPartialDateTime(x => x.Year = null), because: "Two PartialDateTime instances created from the same DateTime but with year null should be equal");
            now.ToPartialDateTime(x => x.Month = null).Should().Be(now.ToPartialDateTime(x => x.Month = null), because: "Two PartialDateTime instances created from the same DateTime but with month null should be equal");
            now.ToPartialDateTime(x => x.DayOfMonth = null).Should().Be(now.ToPartialDateTime(x => x.DayOfMonth = null), because: "Two PartialDateTime instances created from the same DateTime but with day of month null should be equal");
            now.ToPartialDateTime(x => x.Hour = null).Should().Be(now.ToPartialDateTime(x => x.Hour = null), because: "Two PartialDateTime instances created from the same DateTime but with hour null should be equal");
            now.ToPartialDateTime(x => x.Minute = null).Should().Be(now.ToPartialDateTime(x => x.Minute = null), because: "Two PartialDateTime instances created from the same DateTime but with minute null should be equal");
            now.ToPartialDateTime(x => x.Second = null).Should().Be(now.ToPartialDateTime(x => x.Second = null), because: "Two PartialDateTime instances created from the same DateTime but with second null should be equal");
            now.ToPartialDateTime(x => x.Millisecond = null).Should().Be(now.ToPartialDateTime(x => x.Millisecond = null), because: "Two PartialDateTime instances created from the same DateTime but with millisecond null should be equal");
        }

        [Fact(DisplayName = "PartialDateTime Comparisons Work As Expected")]
        public void PartialDateTime_Comparisons_Work_As_Expected()
        {
            (new PartialDateTime() < new PartialDateTime()).Should().BeFalse(because: "A default PartialDateTime is not less than another default PartialDateTime");
            (new PartialDateTime() <= new PartialDateTime()).Should().BeTrue(because: "A default PartialDateTime is not less BUT equal to another default PartialDateTime");
            (new PartialDateTime() > new PartialDateTime()).Should().BeFalse(because: "A default PartialDateTime is not more than another default PartialDateTime");
            (new PartialDateTime() >= new PartialDateTime()).Should().BeTrue(because: "A default PartialDateTime is not more BUT equal to another default PartialDateTime");
        }

        [Fact(DisplayName = "PartialDateTime To Minimum DateTime Conversion Work As Expected")]
        public void PartialDateTime_To_Minimum_DateTime_Conversion_Work_As_Expected()
        {
            PartialDateTime whenever = new PartialDateTime();
            whenever.ToMinimumDateTime().Should().BeNull(because: "Whenever can't be determined to any datetime");

            DateTime now = DateTime.UtcNow;

            whenever.OnYear(now.Year).ToMinimumDateTime().Should().Be(new DateTime(now.Year, 1, 1, 0, 0, 0, 0, now.Kind), because: "Year only should be minimum datetime of that year");
            whenever.OnMonth(now.Month).ToMinimumDateTime().Should().Be(new DateTime(now.Year, now.Month, 1, 0, 0, 0, 0, now.Kind), because: "Month only should be minimum day of that month and current year and minimum time");
            whenever.OnDayOfMonth(now.Day).ToMinimumDateTime().Should().Be(new DateTime(now.Year, 1, now.Day, 0, 0, 0, 0, now.Kind), because: "DayOfMonth only should be minimum month and current year and minimum time");
            whenever.OnHour(now.Hour).ToMinimumDateTime().Should().Be(new DateTime(now.Year, 1, 1, now.Hour, 0, 0, 0, now.Kind), because: "Hour only should be minimum date on current year and minimum time of that hour");
            whenever.OnMinute(now.Minute).ToMinimumDateTime().Should().Be(new DateTime(now.Year, 1, 1, 0, now.Minute, 0, 0, now.Kind), because: "Minute only should be minimum date on current year and minimum hour and minimum time of that minute");
            whenever.OnSecond(now.Second).ToMinimumDateTime().Should().Be(new DateTime(now.Year, 1, 1, 0, 0, now.Second, 0, now.Kind), because: "Second only should be minimum date on current year and minimum hour and minimum minute and that given second");
            whenever.OnMillisecond(now.Millisecond).ToMinimumDateTime().Should().Be(new DateTime(now.Year, 1, 1, 0, 0, 0, now.Millisecond, now.Kind), because: "Millisecond only should be minimum date on current year and minimum time and that given millisecond");

            now.ToDateOnlyPartialDateTime().ToMinimumDateTime().Should().Be(new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0, now.Kind), because: "Date only should be minimum time of that date");
            now.ToTimeOnlyPartialDateTime().ToMinimumDateTime().Should().Be(new DateTime(now.Year, 1, 1, now.Hour, now.Minute, now.Second, now.Millisecond, now.Kind), because: "Time only should be minimum date of that time");

            whenever.OnYear(now.Year).OnMonth(now.Month).ToMinimumDateTime().Should().Be(new DateTime(now.Year, now.Month, 1, 0, 0, 0, 0, now.Kind), because: "Month/year should be minimum day and time of that month/year");

            whenever.OnDayOfMonth(now.Day).OnMonth(now.Month).ToMinimumDateTime().Should().Be(new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0, now.Kind), because: "Day/Month should be minimum time of that day/month on the current year");
        }

        [Fact(DisplayName = "PartialDateTime To Maximum DateTime Conversion Work As Expected")]
        public void PartialDateTime_To_Maximum_DateTime_Conversion_Work_As_Expected()
        {
            PartialDateTime whenever = new PartialDateTime();
            whenever.ToMaximumDateTime().Should().BeNull(because: "Whenever can't be determined to any datetime");

            DateTime now = DateTime.UtcNow;

            whenever.OnYear(now.Year).ToMaximumDateTime().Should().Be(new DateTime(now.Year, 12, DateTime.DaysInMonth(now.Year, now.Month), 23, 59, 59, 999, now.Kind), because: "Year only should be maximum datetime of that year");
            whenever.OnMonth(now.Month).ToMaximumDateTime().Should().Be(new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month), 23, 59, 59, 999, now.Kind), because: "Month only should be maximum day of that month and current year and maximum time");
            whenever.OnDayOfMonth(now.Day).ToMaximumDateTime().Should().Be(new DateTime(now.Year, 12, now.Day, 23, 59, 59, 999, now.Kind), because: "DayOfMonth only should be maximum month and current year and maximum time");
            whenever.OnHour(now.Hour).ToMaximumDateTime().Should().Be(new DateTime(now.Year, 12, DateTime.DaysInMonth(now.Year, now.Month), now.Hour, 59, 59, 999, now.Kind), because: "Hour only should be maximum date on current year and maximum time of that hour");
            whenever.OnMinute(now.Minute).ToMaximumDateTime().Should().Be(new DateTime(now.Year, 12, DateTime.DaysInMonth(now.Year, now.Month), 23, now.Minute, 59, 999, now.Kind), because: "Minute only should be maximum date on current year and maximum hour and maximum time of that minute");
            whenever.OnSecond(now.Second).ToMaximumDateTime().Should().Be(new DateTime(now.Year, 12, DateTime.DaysInMonth(now.Year, now.Month), 23, 59, now.Second, 999, now.Kind), because: "Second only should be maximum date on current year and maximum hour and maximum minute and that given second");
            whenever.OnMillisecond(now.Millisecond).ToMaximumDateTime().Should().Be(new DateTime(now.Year, 12, DateTime.DaysInMonth(now.Year, now.Month), 23, 59, 59, now.Millisecond, now.Kind), because: "Millisecond only should be maximum date on current year and maximum time and that given millisecond");

            now.ToDateOnlyPartialDateTime().ToMaximumDateTime().Should().Be(new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999, now.Kind), because: "Date only should be maximum time of that date");
            now.ToTimeOnlyPartialDateTime().ToMaximumDateTime().Should().Be(new DateTime(now.Year, 12, DateTime.DaysInMonth(now.Year, now.Month), now.Hour, now.Minute, now.Second, now.Millisecond, now.Kind), because: "Time only should be maximum date of that time");

            whenever.OnYear(now.Year).OnMonth(now.Month).ToMaximumDateTime().Should().Be(new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month), 23, 59, 59, 999, now.Kind), because: "Month/year should be maximum day and time of that month/year");

            whenever.OnDayOfMonth(now.Day).OnMonth(now.Month).ToMaximumDateTime().Should().Be(new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999, now.Kind), because: "Day/Month should be maximum time of that day/month on the current year");
        }

        [Fact(DisplayName = "PartialDateTime To DateTime Conversion Work As Expected")]
        public void PartialDateTime_To_DateTime_Conversion_Work_As_Expected()
        {
            PartialDateTime whenever = new PartialDateTime();
            whenever.ToDateTime().Should().BeNull(because: "Whenever can't be determined to any datetime");

            DateTime now = DateTime.UtcNow;

            whenever.OnYear(now.Year).ToDateTimeAtStartOfDay().Should().Be(new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0, DateTimeKind.Utc), because: "Year only at start of day is current date with minimum time");
            whenever.OnYear(now.Year).ToDateTimeAtEndOfDay().Should().Be(new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999, DateTimeKind.Utc), because: "Year only at end of day is current date with maximum time");
            whenever.OnMonth(now.Month).ToDateTimeAtStartOfDay().Should().Be(new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0, DateTimeKind.Utc), because: "Month only at start of day is current date on given month with minimum time");
            whenever.OnMonth(now.Month).ToDateTimeAtEndOfDay().Should().Be(new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999, DateTimeKind.Utc), because: "Month only at end of day is current date on given month with maximum time");
            whenever.OnDayOfMonth(now.Day).ToDateTimeAtStartOfDay().Should().Be(new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0, DateTimeKind.Utc), because: "DayOfMonth only at start of day is current date on given month with minimum time");
            whenever.OnDayOfMonth(now.Day).ToDateTimeAtEndOfDay().Should().Be(new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999, DateTimeKind.Utc), because: "DayOfMonth only at end of day is current date on given day of month with maximum time");
            whenever.OnHour(now.Hour).ToDateTimeAtStartOfDay().Should().Be(new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, 0, DateTimeKind.Utc), because: "Hour only at start of day is current date on given hour with minimum time");
            whenever.OnHour(now.Hour).ToDateTimeAtEndOfDay().Should().Be(new DateTime(now.Year, now.Month, now.Day, now.Hour, 59, 59, 999, DateTimeKind.Utc), because: "Hour only at end of day is current date on given hour with maximum time");
            whenever.OnMinute(now.Minute).ToDateTimeAtStartOfDay().Should().Be(new DateTime(now.Year, now.Month, now.Day, 0, now.Minute, 0, 0, DateTimeKind.Utc), because: "Minute only at start of day is current date on given minute with minimum time");
            whenever.OnMinute(now.Minute).ToDateTimeAtEndOfDay().Should().Be(new DateTime(now.Year, now.Month, now.Day, 23, now.Minute, 59, 999, DateTimeKind.Utc), because: "Minute only at end of day is current date on given minute with maximum time");
            whenever.OnSecond(now.Second).ToDateTimeAtStartOfDay().Should().Be(new DateTime(now.Year, now.Month, now.Day, 0, 0, now.Second, 0, DateTimeKind.Utc), because: "Second only at start of day is current date on given second with minimum time");
            whenever.OnSecond(now.Second).ToDateTimeAtEndOfDay().Should().Be(new DateTime(now.Year, now.Month, now.Day, 23, 59, now.Second, 999, DateTimeKind.Utc), because: "Second only at end of day is current date on given second with maximum time");
            whenever.OnMillisecond(now.Millisecond).ToDateTimeAtStartOfDay().Should().Be(new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, now.Millisecond, DateTimeKind.Utc), because: "Millisecond only at start of day is current date on given millisecond with minimum time");
            whenever.OnMillisecond(now.Millisecond).ToDateTimeAtEndOfDay().Should().Be(new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, now.Millisecond, DateTimeKind.Utc), because: "Millisecond only at end of day is current date on given millisecond with maximum time");
        }

        [Fact(DisplayName = "PartialDateTime IsMatchingDateTime Works As Expected")]
        public void PartialDateTime_IsMatchingDateTime_Works_As_Expected()
        {
            DateTime now = DateTime.UtcNow;

            new PartialDateTime().IsMatchingDateTime(null).Should().BeFalse(because: "NULL date doesn't match anything");
            ((null as PartialDateTime) == (null as DateTime?)).Should().BeTrue(because: "They're both NULL");
            ((null as PartialDateTime) == (now as DateTime?)).Should().BeFalse(because: "partial is NULL");

            PartialDateTime partial = new PartialDateTime();
            
            partial.IsMatchingDateTime(now).Should().BeTrue(because: "Whenever includes any datetime");

            partial.OnYear(now.Year).IsMatchingDateTime(now).Should().BeTrue(because: "Year only should match any datetime on that year");
            partial.OnMonth(now.Month).IsMatchingDateTime(now).Should().BeTrue(because: "Month only should match any datetime on that month");
            partial.OnDayOfMonth(now.Day).IsMatchingDateTime(now).Should().BeTrue(because: "DayOfMonth only should match any datetime on that day");
            partial.OnHour(now.Hour).IsMatchingDateTime(now).Should().BeTrue(because: "Hour only should match any datetime on that hour");
            partial.OnMinute(now.Minute).IsMatchingDateTime(now).Should().BeTrue(because: "Minute only should match any datetime on that minute");
            partial.OnSecond(now.Second).IsMatchingDateTime(now).Should().BeTrue(because: "Second only should match any datetime on that second");
            partial.OnMillisecond(now.Millisecond).IsMatchingDateTime(now).Should().BeTrue(because: "Millisecond only should match any datetime on that millisecond");

            partial.OnYear(now.Year).OnMonth(now.Month).IsMatchingDateTime(now).Should().BeTrue(because: "Month/year should match any datetime on that month/year");

            partial.OnYear(now.Year).OnMonth(now.Month).OnDayOfMonth(now.Day).IsMatchingDateTime(now).Should().BeTrue(because: "Month/year/day should match any datetime on that month/year/day");

            partial.OnHour(now.Hour).OnMinute(now.Minute).IsMatchingDateTime(now.AddMonths(1)).Should().BeTrue(because: "Hour/minute only partial matches any date within that hour/minute");

            partial.OnWeekDays(now.DayOfWeek).IsMatchingDateTime(now).Should().BeTrue(because: "Day of week should match any datetime on that day of week");
            partial.OnWeekDays(now.DayOfWeek).IsMatchingDateTime(now.AddDays(1)).Should().BeFalse(because: "Day of week should not match any datetime not on that day of week");
        }
    }
}
