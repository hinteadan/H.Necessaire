namespace H.Necessaire
{
    public class TaggedValue<TValue> : IStringIdentity
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TValue Value { get; set; }
    }
}
