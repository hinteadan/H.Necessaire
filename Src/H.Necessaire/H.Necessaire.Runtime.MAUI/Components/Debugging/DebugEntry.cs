namespace H.Necessaire.Runtime.MAUI.Components.Debugging
{
    struct DebugEntry : IStringIdentity
    {
        public string ID { get; set; }
        public TimeSpan Duration { get; set; }

        public override string ToString()
        {
            return $"{Duration}: {ID}";
        }
    }
}
