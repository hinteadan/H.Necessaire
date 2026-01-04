namespace H.Necessaire
{
    public class ReleaseVersion : TaggedValue<Version>
    {
        public ReleaseVersion() { }
        internal ReleaseVersion(TaggedValue<Version> taggedValue) : this()
        {
            if (taggedValue is null) throw new OperationResultException("The provided tagged value is null");
            if (taggedValue.Value is null) throw new OperationResultException("The provided tagged value version is null");

            ID = taggedValue.ID;
            Name = taggedValue.Name;
            Description = taggedValue.Description;
            Notes = taggedValue.Notes;
            Value = taggedValue.Value;
        }

        public Version Version => Value;

        public new ReleaseVersion Note(params Note[] notes)
        {
            base.Note(notes);
            return this;
        }
        public new ReleaseVersion Describe(string description)
        {
            base.Describe(description);
            return this;
        }
    }
}
