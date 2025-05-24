namespace H.Necessaire
{
    public class TaggedValue<TValue> : IStringIdentity
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Note[] Notes { get; set; }
        public TValue Value { get; set; }

        public TaggedValue<TValue> Note(params Note[] notes)
        {
            Notes = Notes.Push(notes);
            return this;
        }
        public TaggedValue<TValue> Describe(string description)
        {
            Description = description;
            return this;
        }

        public override string ToString()
        {
            if (Name.IsEmpty())
                return Value?.ToString();

            return $"{Name} ({Value})";
        }


        public static implicit operator TValue(TaggedValue<TValue> taggedValue) => taggedValue.Value;
    }
}
