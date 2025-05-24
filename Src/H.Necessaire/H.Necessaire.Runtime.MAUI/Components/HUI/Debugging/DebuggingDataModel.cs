namespace H.Necessaire.Runtime.MAUI.Components.HUI.Debugging
{
    class DebuggingDataModel
    {
        [DisplayLabel("Boolean Control")]
        public bool IsAwesome { get; set; }

        [DisplayLabel("Default Text Control")]
        public string SomeText { get; set; }

        [DisplayLabel("Selection Control")]
        public string Selection { get; set; }

        public Note Note { get; set; }

        public decimal DecimalNumber { get; set; }
        public int IntegerNumber { get; set; }

        public NumberInterval NumberInterval { get; set; }
        public TimeSpan TimeSpan { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly Time { get; set; }

        public DateTime DateTime { get; set; }

        public PeriodOfTime PeriodOfTime { get; set; }

        public PartialDateTime PartialDateTime { get; set; }
    }
}
