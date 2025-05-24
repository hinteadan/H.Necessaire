namespace H.Necessaire.Runtime.UI
{
    public enum HUIPresentationType
    {
        Custom = 0,

        Boolean = 1,

        Selection = 10,

        Text = 100,
        Note = 101,

        Number = 200,
        NumberInterval = 201,

        TimeSpan = 290,

        Date = 300,
        Time = 301,
        DateAndTime = 302,
        PeriodOfTime = 303,
        PartialDateTime = 304,
        PartialPeriodOfTime = 305,
        ApproximatePeriodOfTime = 306,

        Collection = 1000,


        SubViewModel = 99999,
    }
}